using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Services
{
    public class DbService : IDbService, IDisposable
    {
        public DbService(Dapper.IDatabase connection, IDbTransaction transaction)
        {
            Connection = connection;
            Transaction = transaction;
            Errors = new List<string>();
        }
        public DbService(IDatabase connection)
            : this(connection, null)
        {
        }

        public DbService()
            : this(null, null)
        {
        }

        public IDatabase Connection
        {
            get;
            set;
        }
        public System.Data.IDbTransaction Transaction
        {
            get;
            set;
        }

        public void Dispose()
        {
            if (Connection != null)
                Connection.Dispose();
        }


        public bool BeginTransaction()
        {
            if (Connection != null)
            {
                Connection.BeginTransaction();
                return true;
            }
            return false;
        }

        public bool CommitTransaction()
        {
            if (Connection != null)
            {
                Connection.CommitTransaction();
                return true;
            }
            return false;
        }

        public bool RollbackTransaction()
        {
            if (Connection != null)
            {
                Connection.RollbackTransaction();
                return true;
            }
            return false;
        }


        public List<string> Errors
        {
            get;
            set;
        }
    }
}
