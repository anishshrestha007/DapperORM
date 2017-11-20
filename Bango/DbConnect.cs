using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using System.Configuration;
using LightInject;
namespace Bango
{
    public class DbConnect : IDisposable
    {
        //public MyroDatabase DB { get; private set; }
        public IDatabase DB { get; private set; }
        public IDbConnection Connection { get; set; }
        public DbConnect()
            : this(ConfigurationManager.ConnectionStrings[App.Config.DbConnectionString].ConnectionString)
        {
        }
        public DbConnect(string connectionString)
        {
            Init(connectionString);
        }
        //public TDatabase Init(TDatabase db, IDbConnection connection, string connectionString)
        public bool Init(string connectionString)
        {
            Connection = App.Container.GetInstance<IDbConnection>();
            Connection.ConnectionString = connectionString;
            Connection.Open();
            if (Connection.State == ConnectionState.Open)
            {
                DB = App.Container.GetInstance<Dapper.IDatabase>();
                DB.InitDatabase(Connection, commandTimeout: 30);
                return true;
                
            }
            return false;
        }

        public ITable<TModel, int?> GetModelTable<TModel>()
            where TModel : Models.IModel
        {
            return DB.GetTable<TModel>(Models.ModelService.GetTableName(typeof(TModel)));
            //return new DB.Table<TModel>(DB, string.Empty);
        }

        public ITable<TModel, TKey> GetModelTable<TModel, TKey>()
            where TModel : Models.IModel
        {
            return DB.GetTable<TModel, TKey>(Models.ModelService.GetTableName(typeof(TModel)));
            //return new DB.Table<TModel>(DB, string.Empty);
        }

        void IDisposable.Dispose()
        {
            if (Connection != null)
                Connection.Dispose();
            if (DB != null)
                DB.Dispose();
        }
    }

    public class MyroDatabase : NpgsqlDatabase<MyroDatabase>
    {
    }
}
