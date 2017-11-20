using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;
using Bango.Models;

namespace Bango.Helpers
{
    public class DbHelper : Bango.Helpers.IDbHelper
    {
        public IDbTransaction Trans { get; set; }
        public IDbConnection Connection { get; set; }

        public DbHelper(IDbConnection connection, IDbTransaction trans)
        {
            Trans = trans;
            Connection = connection;
        }

        public DbHelper()
            : this(App.DB.GetConnection(), null)
        {

        }

        public IDbCommand GetCommand()
        {
            return null;
            //return Connection.GetCommand();
        }

        public IDbCommand GetCommand(string commandText)
        {
            return null;
            //return Connection.GetCommand(commandText);
        }

        public IDbDataParameter CreateParameter(PropertyInfo prop, ModelBase model)
        {
            return null;
            //return Connection.CreateParameter(Connection.GetDbType(prop.PropertyType), prop.Name, prop.GetValue(model));
        }

        public IDbDataParameter GetCurrentUserParam(string paramName)
        {
            return null;
            //return Connection.CreateParameter(DbType.Int32, paramName, Bango.GetCurrentUserId());
        }

        public string ParamPrefix
        {
            get
            {
                return string.Empty;
            }
        }

        
    }
}
