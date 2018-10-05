﻿using CustomerManager.Data;
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

        public static string DatabaseName { get; set; }
        public static string DataSource { get; set; }

        public static void Init()
        {
            Console.WriteLine("[DB] Initializing Database..");

            try
            {
                using (var connection = CreateSqlConnection("", "", "", DataSource, true))
                {

                    connection.Open();

                    CheckDatabase(connection);

                    CheckTables(connection);

                }
            }
            catch
            {
                Console.WriteLine("[DB] WRONG DATASOURCE !!!");
            }


        }

        public static void Reset()
        {
            Console.WriteLine("Resetting database... ");

            Delete(DeleteType.Database, DatabaseName);

            Console.WriteLine("Successful!");

        }

        private static SqlConnection CreateSqlConnection(string user, string password, string initialCatalog, string dataSource, bool integratedSecurity)
        {

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            if (!integratedSecurity && user.Length != 0 && password.Length != 0)
            {
                builder.UserID = user;
                builder.Password = password;
            }

            builder.IntegratedSecurity = integratedSecurity;

            if (initialCatalog.Length != 0)
                builder.InitialCatalog = initialCatalog;
            if (dataSource.Length != 0)
                builder.DataSource = dataSource;

            return new SqlConnection(builder.ConnectionString);

        }

        public static void DeleteCustomer(int id, bool shippingAddresses)
        {

            if (!DataManager.Contains(id))
            {
                Console.WriteLine("[DB] User does not exist!");
                return;
            }

            Console.Write("[DB] Deleting customer n°" + id + (shippingAddresses ? " with Shipping Addresses" : "") + "... ");

            using (var connection = CreateSqlConnection("", "", DatabaseName, DataSource, true))
            {

                connection.Open();

                // Delete consumer
                using (var cmd = new SqlCommand("delete from Customers where id=@id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                if (shippingAddresses)
                {

                    // Delete shippingaddresses
                    using (var cmd = new SqlCommand("delete from ShippingAddresses where customerid=@customerId", connection))
                    {
                        cmd.Parameters.AddWithValue("@customerId", id);
                        cmd.ExecuteNonQuery();
                    }

                }

                Console.WriteLine("Success!");

            }
        }

        public static void LoadData()
        {

            int addresses = 0, customers = 0;

            Console.WriteLine("[DB] Loading data from database... ");

            using (var connection = CreateSqlConnection("", "", DatabaseName, DataSource, true))
            {

                connection.Open();

                var cmd = new SqlCommand("select Customers.id,firstname,name,dateofbirth,phonenumber,email,ShippingAddresses.address, ShippingAddresses.postalcode from (Customers left join ShippingAddresses on Customers.id=ShippingAddresses.customerid)", connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        // TODO: maybe check if id,firstname,name,... are not null
                        int id = (int)reader["id"];
                        var address = reader["address"];
                        var postalCode = reader["postalcode"];
                        Customer customer;

                        // Console.WriteLine("["+ id + "] " +reader["firstname"].ToString() + " " + reader["name"].ToString() + " " + ((DateTime)reader["dateofbirth"]).ToShortDateString() + " " + reader["email"].ToString() + " " + address + " " + postalCode);


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



                        if (customer != null && address != DBNull.Value && postalCode != DBNull.Value && DataManager.AddWithoutDoubles(customer, new ShippingAddress(id, address.ToString(), postalCode.ToString())) != null)
                            addresses++;

                    }
                }

            }

            if (customers != 0)
                Console.WriteLine("[DB] " + customers + " customers found!");
            else
                Console.WriteLine("[DB] No customers found! :(");

            if (addresses != 0)
                Console.WriteLine("[DB] " + addresses + " addresses found!");
            else
                Console.WriteLine("[DB] No addresses found! :(");

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

            Console.WriteLine("[DB] Checking tables...");

            Console.Write("[DB] Table 'Customers': ");
            if (!TableExists("Customers", connection))
            {
                Console.WriteLine("Creating...");

                using (var create = new SqlCommand("create table Customers(id int not null identity,firstname nvarchar(max),name nvarchar(max),dateofbirth date,phonenumber nvarchar(max),email nvarchar(max),PRIMARY KEY(id))", connection))
                {
                    create.ExecuteNonQuery();
                    Console.WriteLine("Table created!");
                }

            }
            else
            {
                Console.WriteLine("OK!");
            }

            Console.Write("[DB] Table 'ShippingAddresses': ");
            if (!TableExists("ShippingAddresses", connection))
            {
                Console.WriteLine("Creating...");

                using (var create = new SqlCommand("create table ShippingAddresses(id int not null identity,customerid int,address nvarchar(max),postalcode nvarchar(max),PRIMARY KEY(id))", connection))
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


        public static void Delete(DeleteType type, string name) // Isnt working (DDL OP)
        {

            Console.Write("[DB] Trying to delete " + type.ToString().ToLower() + " " + name + "... ");

            using (var connection = CreateSqlConnection("", "", type == DeleteType.Table ? DatabaseName : "", DataSource, true))
            {

                connection.Open();

                using (var cmd = new SqlCommand("drop " + type.ToString() + " " + name, connection)) // waiting to find another way
                {

                    //cmd.Parameters.AddWithValue("@name", name);

                    if (cmd.ExecuteNonQuery() != 0)
                        Console.WriteLine("Success!");
                    else
                        Console.WriteLine("ERROR!");

                }


            }

        }

        public static void CheckDatabase(SqlConnection connection)
        {

            Console.Write("[DB] Checking if " + DatabaseName + " database exists...");

            using (var cmd = new SqlCommand("SELECT db_id(@databaseName)", connection))
            {

                cmd.Parameters.AddWithValue("@databaseName", DatabaseName);

                if (cmd.ExecuteScalar() == DBNull.Value)
                {
                    Console.WriteLine(" Creating...");

                    using (var create = new SqlCommand("Create database " + DatabaseName, connection)) //TODO: sql injection?
                    {
                        //create.Parameters.AddWithValue("@databaseName", DatabaseName);  // Isnt working (DDL OP)
                        create.ExecuteNonQuery();
                    }

                }
                else
                {
                    Console.WriteLine(" Yep!");
                }

            }

            connection.ChangeDatabase(DatabaseName);

        }

        public static int SaveCustomersToDB(List<Customer> customers)
        {
            if (customers.Count == 0) return 0;

            int count = 0;

            using (SqlConnection connection = CreateSqlConnection("", "", DatabaseName, DataSource, true))
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

        public static int SaveShippingAddressesToDB(List<ShippingAddress> shippingAddresses)
        {
            if (shippingAddresses.Count == 0) return 0;

            int count = 0;

            using (SqlConnection connection = CreateSqlConnection("", "", DatabaseName, DataSource, true))
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

    }


    enum DeleteType
    {
        Table,
        Database
    }

}
