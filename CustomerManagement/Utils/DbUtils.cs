using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace CustomerManagement.Utils
{
    public class DbUtils
    {

        public static void LoadAndExecuteSqlScript(string path, SqlConnection connection)
        {

            var server = new Server(new ServerConnection(connection));
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(path))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadToEnd();
                server.ConnectionContext.ExecuteNonQuery(result);
            }

        }

        public static SqlConnection CreateSqlConnection(string initialCatalog, string dataSource)
        {

            var builder = new SqlConnectionStringBuilder
            {
                IntegratedSecurity = true
            };

            if (!string.IsNullOrEmpty(initialCatalog))
                builder.InitialCatalog = initialCatalog;
            if (!string.IsNullOrEmpty(dataSource))
                builder.DataSource = dataSource;

            return new SqlConnection(builder.ConnectionString);

        }

        public static void DbLogWrite(string log)
        {
            Console.Write($"[DB] {log}");
        }

        public static void DbLogWriteLine(string log)
        {
            Console.WriteLine($"[DB] {log}");
        }

    }
}
