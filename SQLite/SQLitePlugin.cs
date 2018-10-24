using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CustomerManagement.Data;
using CustomerManagement.Interfaces;
using CustomerManagement.Utils;
using CustomerManager.Data;

namespace SQLite
{
    public class SQLitePlugin : IPlugin
    {
        private string DataSource { get; set; }

        public string GetName()
        {
            return "SQLite";
        }

        public string GetDataSource()
        {
            return DataSource;
        }

        public bool IsNeedingFile()
        {
            return true;
        }

        public string GetFileExtension()
        {
            return ".db";
        }

        public void SetDataSource(string dataSource)
        {
            DataSource = dataSource;
        }

        public void Init()
        {

            DbUtils.DbLogWriteLine("Initializing Database..");

            try
            {

                if (!File.Exists(DataSource))
                    SQLiteConnection.CreateFile(DataSource);

                using (var connection = CreateConnection())
                {
                    DbUtils.DbLogWriteLine("Checking database & tables...");

                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {

                        // Create tables
                        using (var cmd = new SQLiteCommand("create table if not exists Customers(id integer primary key, firstname text, name text, dateofbirth date, phonenumber text, email text)", connection, transaction))
                            cmd.ExecuteNonQuery();

                        using (var cmd = new SQLiteCommand("create table if not exists ShippingAddresses(id integer primary key,customerid integer,address text,postalcode text,foreign key(customerid) references Customers(id)) ", connection, transaction))
                            cmd.ExecuteNonQuery();

                        transaction.Commit();
                        Console.WriteLine("Success.");

                    }


                }
            }
            catch (Exception e)
            {
                DbUtils.DbLogWriteLine($"Error occurred: {e.Message}");
            }


        }

        public bool Reset()
        {
            DbUtils.DbLogWriteLine("Resetting database... ");

            using (var connection = CreateConnection())
            {
                Init();
                return true;
            }

        }

        public void DeleteCustomer(int id)
        {
            DbUtils.DbLogWrite($"Deleting customer with id={ id}... ");

            using (var connection = CreateConnection())
            {

                connection.Open();



                using (var transaction = connection.BeginTransaction())
                {

                    // Delete shippingaddresses
                    using (var cmd = new SQLiteCommand("delete from ShippingAddresses where customerid=@customerId",
                        connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@customerId", id);
                        cmd.ExecuteNonQuery();
                    }

                    // Delete consumer
                    using (var cmd = new SQLiteCommand("delete from Customers where id=@id", connection,
                        transaction))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }



                    transaction.Commit();
                    Console.WriteLine("Success.");
                }


            }
        }

        public void DeleteShippingAddress(int id)
        {
            DbUtils.DbLogWrite($"Deleting shipping address n°{ id}... ");

            using (var connection = CreateConnection())
            {

                connection.Open();

                // Delete shippingaddresses
                using (var cmd = new SQLiteCommand("delete from ShippingAddresses where id=@id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Success.");
                }

            }
        }

        public Customer GetCustomer(int id)
        {
            var customer = new Customer();

            using (var connection = CreateConnection())
            {

                connection.Open();

                var cmd = new SQLiteCommand("select firstname, name, dateofbirth, phonenumber, email from Customers where id=@id", connection);
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {

                    if (!reader.Read()) return new Customer();

                    customer.Id = id;
                    customer.FirstName = reader["firstname"].ToString();
                    customer.Name = reader["name"].ToString();
                    customer.DateOfBirth = (DateTime)reader["dateofbirth"];
                    customer.PhoneNumber = reader["phonenumber"].ToString();
                    customer.Email = reader["email"].ToString();

                }

            }

            return customer;
        }

        public List<ShippingAddress> GetShippingAddresses(int customerId)
        {

            var list = new List<ShippingAddress>();

            using (var connection = CreateConnection())
            {

                connection.Open();

                var cmd = new SQLiteCommand("select id, address, postalcode from ShippingAddresses where customerId=@customerId", connection);
                cmd.Parameters.AddWithValue("@customerId", customerId);

                using (var reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {

                        var id = (int)reader["id"];
                        var address = reader["address"].ToString();
                        var postalCode = reader["postalcode"].ToString();
                        list.Add(new ShippingAddress(id, customerId, address, postalCode));

                    }
                }

            }

            return list;

        }

        public List<Customer> LoadData()
        {

            var list = new List<Customer>();

            int addresses = 0, customers = 0;

            DbUtils.DbLogWriteLine("Loading data from database... ");

            using (var connection = CreateConnection())
            {

                connection.Open();

                var cmd = new SQLiteCommand("select Customers.id,ShippingAddresses.id as addressId,firstname,name,dateofbirth,phonenumber,email,ShippingAddresses.address, ShippingAddresses.postalcode from (Customers left join ShippingAddresses on Customers.id=ShippingAddresses.customerid)", connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        var id = int.Parse(reader["id"].ToString());
                        var address = reader["address"];
                        var postalCode = reader["postalcode"];
                        Customer customer;

                        if (!DataManager.Contains(list, id))
                        {

                            customer = new Customer(reader["firstname"].ToString(), reader["name"].ToString(), (DateTime)reader["dateofbirth"], reader["phonenumber"].ToString(), reader["email"].ToString())
                            {
                                Id = id
                            };
                            list.Add(customer);
                            customers++;
                        }
                        else
                        {
                            customer = DataManager.Find(list, id);
                        }

                        if (customer != null && address != DBNull.Value && postalCode != DBNull.Value && DataManager.AddWithoutDoubles(customer, new ShippingAddress((int)reader["addressId"], id, address.ToString(), postalCode.ToString())) != null)
                            addresses++;

                    }
                }

            }

            if (customers != 0)
                DbUtils.DbLogWriteLine($"{customers} customer(s) found.");
            else
                DbUtils.DbLogWriteLine("No customers found. :(");

            if (addresses != 0)
                DbUtils.DbLogWriteLine($"{customers} addresse(s) found.");
            else
                DbUtils.DbLogWriteLine("No addresses found. :(");

            return list;
        }


        public int SaveCustomersToDb(List<Customer> customers)
        {

            if (customers.Count == 0) return 0;

            var count = 0;

            using (var connection = CreateConnection())
            {

                connection.Open();

                foreach (var customer in customers)
                {

                    if (customer.Id != 0) continue;

                    using (var check = new SQLiteCommand("select exists(select id from Customers where firstname=@firstName and name=@name and dateofbirth=@dateofbirth)", connection))
                    using (var cmd = new SQLiteCommand("insert into Customers(firstname,name,dateofbirth,phonenumber,email) values (@firstname,@name,@dateofbirth,@phonenumber,@email)", connection))
                    using (var select = new SQLiteCommand("select last_insert_rowid()", connection))
                    {
                        check.Parameters.AddWithValue("@firstname", customer.FirstName);
                        check.Parameters.AddWithValue("@name", customer.Name);
                        check.Parameters.AddWithValue("@dateofbirth", customer.DateOfBirth);

                        if (int.TryParse(check.ExecuteScalar().ToString(), out int result) && result != 0) continue;  

                        cmd.Parameters.AddWithValue("@firstname", customer.FirstName);
                        cmd.Parameters.AddWithValue("@name", customer.Name);
                        cmd.Parameters.AddWithValue("@dateofbirth", customer.DateOfBirth);
                        cmd.Parameters.AddWithValue("@phonenumber", customer.PhoneNumber);
                        cmd.Parameters.AddWithValue("@email", customer.Email);
                        cmd.ExecuteNonQuery();

                        var retour = select.ExecuteScalar();
                        if (retour == null) continue;
                        customer.Id = int.Parse(retour.ToString());
                        count++;


                    }

                }

            }

            return count;

        }

        public int SaveShippingAddressesToDb(List<ShippingAddress> shippingAddresses)
        {

            if (shippingAddresses.Count == 0) return 0;

            int count = 0;

            using (var connection = CreateConnection())
            {

                connection.Open();

                foreach (var shippingAddress in shippingAddresses)
                {

                    using (var cmd = new SQLiteCommand("insert into ShippingAddresses(customerid,address,postalcode) values (@customerid,@address,@postalcode) where not exists (select id from ShippingAddresses where customerid=@customerid and address=@address and postalcode=@postalcode)", connection))
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

        public void UpdateCustomer(Customer customer)
        {

            using (var connection = CreateConnection())
            {
                connection.Open();

                using (var cmd = new SQLiteCommand("update Customers set name=@name,firstname=@firstname,dateofbirth=@dateofbirth,phonenumber=@phonenumber,email=@email where id=@id", connection))
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

        public void UpdateShippingAddress(ShippingAddress address)
        {

            using (var connection = CreateConnection())
            {
                connection.Open();

                using (var cmd = new SQLiteCommand("update ShippingAddresses set customerid=@customerid,address=@address,postalcode=@postalcode where id=@id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", address.Id);
                    cmd.Parameters.AddWithValue("@customerid", address.CustomerId);
                    cmd.Parameters.AddWithValue("@address", address.Address);
                    cmd.Parameters.AddWithValue("@postalcode", address.PostalCode);
                    cmd.ExecuteNonQuery();
                }

            }
        }

        private SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection($"Data Source={DataSource};Version=3;");
        }

    }
}
