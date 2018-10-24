using System;

namespace CustomerManagement.Utils
{
    public class DbUtils
    {

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
