using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thor.DataAccess;
using System.Configuration;

namespace Thor.DataAccess
{
    public static class GlobalConfig
    {
        //public static List<IDataConnection> Connections { get; private set; } = new List<IDataConnection>(); - poate vrem sa folosim si fisiere text
        // se face o alta clasa, textconnection.cs si acolo se implementeaza contractul IDataConnection
     //   public static IDataConnection connection;

        public static void InitializeConnection() //bool sql, bool text - optiune de stocare a datelor
        {

        }

        public static string CnnString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}
