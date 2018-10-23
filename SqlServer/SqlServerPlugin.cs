using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerManagement.Data;
using CustomerManagement.Enums;
using CustomerManagement.Interfaces;
using CustomerManagement.Utils;
using CustomerManager.Data;

namespace SqlServer
{

    public class SqlServerPlugin : IPlugin
    {
        private string DataSource { get; set; }
        private string DatabaseName { get; } = "CustomerManager";

        public void DeleteCustomer(int id)
        {
            DbUtils.DbLogWrite($"Deleting customer with id={ id}... ");

            using (var connection = DbUtils.CreateSqlConnection(DatabaseName, DataSource))
            {

                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {

                    // Delete shippingaddresses
                    using (var cmd = new SqlCommand("delete from ShippingAddresses where customerid=@customerId", connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@customerId", id);
                        cmd.ExecuteNonQuery();
                    }

                    // Delete consumer
                    using (var cmd = new SqlCommand("delete from Customers where id=@id", connection, transaction))
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

            using (var connection = DbUtils.CreateSqlConnection(DatabaseName, DataSource))
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

        public Customer GetCustomer(int id)
        {
            var customer = new Customer();

            using (var connection = DbUtils.CreateSqlConnection(DatabaseName, DataSource))
            {

                connection.Open();

                var cmd = new SqlCommand("select firstname, name, dateofbirth, phonenumber, email from Customers where id=@id", connection);
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

        public string GetName()
        {
            return "SqlServer";
        }

        public string GetDataSource()
        {
            return DataSource;
        }

        public void SetDataSource(string dataSource)
        {
            DataSource = dataSource;
        }

        public List<ShippingAddress> GetShippingAddresses(int customerId)
        {

            var list = new List<ShippingAddress>();

            using (var connection = DbUtils.CreateSqlConnection(DatabaseName, DataSource))
            {

                connection.Open();

                var cmd = new SqlCommand("select id, address, postalcode from ShippingAddresses where customerId=@customerId", connection);
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

        public void Init()
        {

            DbUtils.DbLogWriteLine("Initializing Database..");

            try
            {
                using (var connection = DbUtils.CreateSqlConnection("", DataSource))
                {
                    DbUtils.DbLogWriteLine("Checking database & tables...");

                    DbUtils.LoadAndExecuteSqlScript("CustomerManagement.SQL.InitializeDatabase.sql", connection);

                }
            }
            catch (Exception e)
            {
                DbUtils.DbLogWriteLine($"Error occurred: {e.Message}");
            }



        }

        public List<Customer> LoadData()
        {

            var list = new List<Customer>();

            int addresses = 0, customers = 0;

            DbUtils.DbLogWriteLine("Loading data from database... ");

            using (var connection = DbUtils.CreateSqlConnection(DatabaseName, DataSource))
            {

                connection.Open();

                var cmd = new SqlCommand("select Customers.id,ShippingAddresses.id as addressId,firstname,name,dateofbirth,phonenumber,email,ShippingAddresses.address, ShippingAddresses.postalcode from (Customers left join ShippingAddresses on Customers.id=ShippingAddresses.customerid)", connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        var id = (int)reader["id"];
                        var address = reader["address"];
                        var postalCode = reader["postalcode"];
                        Customer customer;

                        if (!DataManager.Contains(list, id))
                        {

                            customer = new Customer(reader["firstname"].ToString(), reader["name"].ToString(), (DateTime)reader["dateofbirth"], reader["phonenumber"].ToString(), reader["email"].ToString())
                            {
                                Id = (int)reader["id"]
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

        public bool Reset()
        {
            DbUtils.DbLogWriteLine("Resetting database... ");

            using (var connection = DbUtils.CreateSqlConnection("", DataSource))
            {
                DbUtils.LoadAndExecuteSqlScript("CustomerManagement.SQL.DropDatabase.sql", connection);
                Init();
                return true;
            }
        }

        public int SaveCustomersToDb(List<Customer> customers)
        {

            if (customers.Count == 0) return 0;

            int count = 0;

            using (var connection = DbUtils.CreateSqlConnection(DatabaseName, DataSource))
            {

                connection.Open();

                foreach (var customer in customers)
                {

                    if (customer.Id != 0) continue;


                    using (var cmd = new SqlCommand("if not exists (select id from Customers where firstname=@firstName and name=@name and dateofbirth=@dateofbirth) begin insert into Customers(firstname,name,dateofbirth,phonenumber,email) output inserted.id values (@firstname,@name,@dateofbirth,@phonenumber,@email) end", connection))
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

        public int SaveShippingAddressesToDb(List<ShippingAddress> shippingAddresses)
        {

            if (shippingAddresses.Count == 0) return 0;

            int count = 0;

            using (var connection = DbUtils.CreateSqlConnection(DatabaseName, DataSource))
            {

                connection.Open();

                foreach (var shippingAddress in shippingAddresses)
                {

                    using (var cmd = new SqlCommand("if not exists (select id from ShippingAddresses where customerid=@customerid and address=@address and postalcode=@postalcode) begin insert into ShippingAddresses(customerid,address,postalcode) values (@customerid,@address,@postalcode) end", connection))
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

            using (var connection = DbUtils.CreateSqlConnection(DatabaseName, DataSource))
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

        public void UpdateShippingAddress(ShippingAddress address)
        {

            using (var connection = DbUtils.CreateSqlConnection(DatabaseName, DataSource))
            {
                connection.Open();

                using (var cmd = new SqlCommand("update ShippingAddresses set customerid=@customerid,address=@address,postalcode=@postalcode where id=@id", connection))
                {
                    cmd.Parameters.AddWithValue("@id", address.Id);
                    cmd.Parameters.AddWithValue("@customerid", address.CustomerId);
                    cmd.Parameters.AddWithValue("@address", address.Address);
                    cmd.Parameters.AddWithValue("@postalcode", address.PostalCode);
                    cmd.ExecuteNonQuery();
                }

            }
        }
    }

}
