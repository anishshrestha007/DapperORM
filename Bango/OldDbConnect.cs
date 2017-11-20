using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using LightInject;
namespace Bango
{
    //public delegate ConnectionBase dlgConnectionObject();
    public class OldDbConnect : IDisposable
    {
        private IDbConnection Connection { get; set; }
        //public static Commands.CommandBase<ConnectionBase> FetchConnectionCmd { get; set; }
        //public static OracleDBConnect Con ;
        public OldDbConnect()
        {
            Connection = null;
        }

        public OldDbConnect(string connectionString)
        {
            Connection = null;
        }

        public IDbConnection GetConnection()
        {
            if (Connection != null)
                return Connection;
            string conString = ConfigurationManager.ConnectionStrings[App.Config.DbConnectionString].ConnectionString;
            return GetConnection(conString);

        }
        public IDbConnection GetConnection(string connectionString)
        {
            if (Connection != null)
                return Connection;
            Connection = App.Container.GetInstance<IDbConnection>();
            Connection.ConnectionString = connectionString;
            Connection.Open();
            return Connection;

        }

        public void Dispose()
        {
            if (Connection != null)
                Connection.Dispose();
            //throw new NotImplementedException();
        }
    }
}
