using CustomerManagement.Data;
using CustomerManager.Data;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManagement.Data
{
    public class DbManager
    {

        public static string DatabaseName { get; set; }
        public static string DataSource { get; set; }

        public static void Init()
        {
            DbLogWriteLine("Initializing Database..");
            DatabaseName = "CustomerManager";

            try
            {
                using (var connection = CreateSqlConnection("", DataSource))
                {

                    DbLogWriteLine("Checking database & tables...");

                    LoadAndExecuteSQLScript("CustomerManagement.SQL.InitializeDatabase.sql", connection);

                    connection.ChangeDatabase("CustomerManager");


                }
            }
            catch (Exception e)
            {
                DbLogWriteLine($"Error occurred: {e.Message}");
            }


        }

        public static void Reset()
        {
            DbLogWriteLine("Resetting database... ");

            using (var connection = CreateSqlConnection("", DataSource))
                LoadAndExecuteSQLScript("CustomerManagement.SQL.DropDatabase.sql", connection);

        }

        public static void UpdateCustomer(Customer customer)
        {

            using (var connection = CreateSqlConnection(DatabaseName, DataSource))
            {
                connection.Open();

                using (var cmd = new SqlCommand("update Customers set name=@name,firstname=@firstname,dateofbirth=@dateofbirth,phonenumber=@phonenumber,email=@email where id=@id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", customer.Id);
                    cmd.Parameters.AddWithValue("@name", customer.Name);
                    cmd.Parameters.AddWithValue("@firstname", customer.FirstName);
                    cmd.Parameters.AddWithValue("@dateofbirth", customer.DateOfBirth);
                    cmd.Parameters.AddWithValue("@phonenumber", customer.PhoneNumber);
                    cmd.Parameters.AddWithValue("@email", customer.Email);
                    cmd.ExecuteNonQuery();
                }

            }

        }

        public static void UpdateShippingAddress(ShippingAddress address)
        {

            using (var connection = CreateSqlConnection(DatabaseName, DataSource))
            {
                connection.Open();

                using (var cmd = new SqlCommand("update ShippingAddresses set customerid=@customerid,address=@address,postalcode=@postalcode where id=@id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", address.Id);
                    cmd.Parameters.AddWithValue("@customerid", address.CustomerId);
                    cmd.Parameters.AddWithValue("@address", address.Address);
                    cmd.Parameters.AddWithValue("@postalcode",address.PostalCode);
                    cmd.ExecuteNonQuery();
                }

            }

        }

        public static void DeleteShippingAddress(int id)
        {

            ShippingAddress address = DataManager.FindShippingAddress(id);

            if (address == null)
            {
                DbLogWriteLine("ShippingAddress does not exist.");
                return;
            }

            DbLogWrite($"Deleting ShippingAddress n°{ id}... ");

            using (var connection = CreateSqlConnection(DatabaseName, DataSource))
            {

                connection.Open();

                // Delete shippingaddresses
                using (var cmd = new SqlCommand("delete from ShippingAddresses where id=@id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Success.");
                }

            }
        }



        public static void DeleteCustomer(int id)
        {

            Customer customer = DataManager.Find(id);

            if (customer == null)
            {
                DbLogWriteLine("User does not exist.");
                return;
            }

            DbLogWrite($"Deleting customer n°{ id}... ");

            using (var connection = CreateSqlConnection(DatabaseName, DataSource))
            {

                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    // Delete consumer
                    using (var cmd = new SqlCommand("delete from Customers where id=@id", connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }

                    // Delete shippingaddresses
                    using (var cmd = new SqlCommand("delete from ShippingAddresses where customerid=@customerId", connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@customerId", id);
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    DataManager.Remove(customer);
                    Console.WriteLine("Success.");
                }

            }
        }

        public static void LoadData()
        {

            int addresses = 0, customers = 0;

            DbLogWriteLine("Loading data from database... ");

            using (var connection = CreateSqlConnection(DatabaseName, DataSource))
            {

                connection.Open();

                var cmd = new SqlCommand("select Customers.id,ShippingAddresses.id as addressId,firstname,name,dateofbirth,phonenumber,email,ShippingAddresses.address, ShippingAddresses.postalcode from (Customers left join ShippingAddresses on Customers.id=ShippingAddresses.customerid)", connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        int id = (int)reader["id"];
                        var address = reader["address"];
                        var postalCode = reader["postalcode"];
                        Customer customer;

                        if (!DataManager.Contains(id))
                        {

                            customer = new Customer(reader["firstname"].ToString(), reader["name"].ToString(), (DateTime)reader["dateofbirth"], reader["phonenumber"].ToString(), reader["email"].ToString())
                            {
                                Id = (int)reader["id"]
                            };
                            DataManager.Customers.Add(customer);
                            customers++;
                        }
                        else
                        {
                            customer = DataManager.Find(id);
                        }

                        if (customer != null && address != DBNull.Value && postalCode != DBNull.Value && DataManager.AddWithoutDoubles(customer, new ShippingAddress((int)reader["addressId"], id, address.ToString(), postalCode.ToString())) != null)
                            addresses++;

                    }
                }

            }

            if (customers != 0)
                DbLogWriteLine($"{customers} customers found.");
            else
                DbLogWriteLine("No customers found. :(");

            if (addresses != 0)
                DbLogWriteLine($"{customers} addresses found.");
            else
                DbLogWriteLine("No addresses found. :(");

        }

        public static bool TableExists(string name, SqlConnection connection)
        {
            using (var cmd = new SqlCommand("select case when exists(select * from information_schema.tables where table_name=@tableName) then 1 else 0 end", connection))
            {
                cmd.Parameters.AddWithValue("@tableName", name);
                return (int)cmd.ExecuteScalar() == 1;
            }

        }

        public static SqlConnection CreateSqlConnection(string initialCatalog, string dataSource)
        {

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                IntegratedSecurity = true
            };

            if (initialCatalog.Length != 0)
                builder.InitialCatalog = initialCatalog;
            if (dataSource.Length != 0)
                builder.DataSource = dataSource;

            return new SqlConnection(builder.ConnectionString);

        }

        public static int SaveShippingAddressesToDB(List<ShippingAddress> shippingAddresses)
        {
            if (shippingAddresses.Count == 0) return 0;

            int count = 0;

            using (SqlConnection connection = CreateSqlConnection(DatabaseName, DataSource))
            {

                connection.Open();

                foreach (ShippingAddress shippingAddress in shippingAddresses)
                {

                    using (SqlCommand cmd = new SqlCommand("if not exists (select id from ShippingAddresses where customerid=@customerid and address=@address and postalcode=@postalcode) begin insert into ShippingAddresses(customerid,address,postalcode) values (@customerid,@address,@postalcode) end", connection))
                    {
                        cmd.Parameters.AddWithValue("@customerid", shippingAddress.CustomerId);
                        cmd.Parameters.AddWithValue("@address", shippingAddress.Address);
                        cmd.Parameters.AddWithValue("@postalcode", shippingAddress.PostalCode);
                        count += cmd.ExecuteNonQuery();
                    }

                }

            }

            return count;

        }


        public static int SaveCustomersToDB(List<Customer> customers)
        {
            if (customers.Count == 0) return 0;

            int count = 0;

            using (SqlConnection connection = CreateSqlConnection(DatabaseName, DataSource))
            {

                connection.Open();

                foreach (Customer customer in customers)
                {

                    if (customer.Id != 0) continue;


                    using (SqlCommand cmd = new SqlCommand("if not exists (select id from Customers where firstname=@firstName and name=@name and dateofbirth=@dateofbirth) begin insert into Customers(firstname,name,dateofbirth,phonenumber,email) output inserted.id values (@firstname,@name,@dateofbirth,@phonenumber,@email) end", connection))
                    {
                        cmd.Parameters.AddWithValue("@firstname", customer.FirstName);
                        cmd.Parameters.AddWithValue("@name", customer.Name);
                        cmd.Parameters.AddWithValue("@dateofbirth", customer.DateOfBirth);
                        cmd.Parameters.AddWithValue("@phonenumber", customer.PhoneNumber);
                        cmd.Parameters.AddWithValue("@email", customer.Email);
                        var retour = cmd.ExecuteScalar();
                        if (retour != null)
                        {
                            customer.Id = (int)retour;
                            count++;
                        }

                    }

                }

            }

            return count;

        }

        public static void LoadAndExecuteSQLScript(string path, SqlConnection connection)
        {

            Server server = new Server(new ServerConnection(connection));
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(path))
            using (var reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                server.ConnectionContext.ExecuteNonQuery(result);
            }

        }

        private static void DbLogWrite(string log)
        {
            Console.Write($"[DB] {log}");
        }

        private static void DbLogWriteLine(string log)
        {
            Console.WriteLine($"[DB] {log}");
        }

    }

    public enum DeleteType
    {
        Table,
        Database
    }
}
