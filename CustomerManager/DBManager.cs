using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManager
{
    class DBManager
    {

        private static String connectionString = @"Server=GRIEVOUS\HISTORIAN;Integrated Security=True;";

        public static void Init()
        {
            Console.WriteLine("Initializing Database..");

            using (var connection = new SqlConnection(connectionString))
            {

                connection.Open();

                CheckDatabase(connection);

                CheckTables(connection);

            }


        }

        public static List<Customer> LoadData()
        {

            List<Customer> customers = new List<Customer>();

            using (var connection = new SqlConnection(connectionString))
            {

                connection.Open();

                var cmd = new SqlCommand("select * from Customers", connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Customer customer = new Customer(reader["firstname"].ToString(), reader["name"].ToString(), (DateTime)reader["dateofbirth"], reader["phonenumber"].ToString(), reader["email"].ToString());
                        customer.Id = (int)reader["id"];
                        customers.Add(customer);
                    }
                }

                connection.Close();

            }
            return customers;
        }

        public static bool TableExists(string name, SqlConnection connection)
        {
            using (var cmd = new SqlCommand("select case when exists(select * from information_schema.tables where table_name=@tableName) then 1 else 0 end", connection))
            {
                cmd.Parameters.AddWithValue("@tableName", name);
                return (int)cmd.ExecuteScalar() == 1;
            }

        }

        public static void CheckTables(SqlConnection connection)
        {

            Console.WriteLine("Checking tables...");

            Console.Write("Table 'Customers': ");
            if (!TableExists("Customers", connection))
            {
                Console.WriteLine("Creating...");

                using (var create = new SqlCommand("CREATE TABLE Customers(id int not null identity,firstname nvarchar(max),name nvarchar(max),dateofbirth date,phonenumber nvarchar(max),email nvarchar(max),PRIMARY KEY(id))", connection))
                {
                    create.ExecuteNonQuery();
                    Console.WriteLine("Table created!");
                }

            }
            else
            {
                Console.WriteLine("OK!");
            }

            Console.Write("Table 'ShippingAddresses': ");
            if (!TableExists("ShippingAddresses", connection))
            {
                Console.WriteLine("Creating...");

                using (var create = new SqlCommand("CREATE TABLE ShippingAddresses(id int not null identity,customerid int,address nvarchar(max),postalcode nvarchar(max),PRIMARY KEY(id))", connection))
                {
                    create.ExecuteNonQuery();
                    Console.WriteLine("Table created!");
                }

            }
            else
            {
                Console.WriteLine("OK!");
            }

        }

        public static void CheckDatabase(SqlConnection connection)
        {

            Console.Write("Checking if CustomerManager database exists...");

            using (var cmd = new SqlCommand("SELECT db_id('CustomerManager')", connection))
            {

                if (cmd.ExecuteScalar() == DBNull.Value)
                {
                    Console.WriteLine(" Creating...");

                    using (var create = new SqlCommand("Create database CustomerManager", connection))
                    {
                        create.ExecuteNonQuery();
                    }

                }
                else
                {
                    Console.WriteLine(" Yep!");
                }

            }

            connection.ChangeDatabase("CustomerManager");

        }

    }
}
