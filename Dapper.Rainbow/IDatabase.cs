using System;
using System.Collections.Generic;
using System.Data;
namespace Dapper
{
    public interface IDatabase
    {
        //IDatabase Init(IDbConnection connection, int commandTimeout);
        void InitDatabase(IDbConnection connection, int commandTimeout);
        ITable<T, int?> GetTable<T>(string likelyTableName); 
        ITable<T, TId> GetTable<T, TId>(string likelyTableName);
        void BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadCommitted);
        void CommitTransaction();
        void Dispose();
        int Execute(string sql, dynamic param = null);
        System.Collections.Generic.IEnumerable<dynamic> Query(string sql, dynamic param = null, bool buffered = true);
        System.Collections.Generic.IEnumerable<T> Query<T>(string sql, dynamic param = null, bool buffered = true);
        System.Collections.Generic.IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, dynamic param = null, System.Data.IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null);
        System.Collections.Generic.IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, dynamic param = null, System.Data.IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null);
        System.Collections.Generic.IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, dynamic param = null, System.Data.IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null);
        System.Collections.Generic.IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, dynamic param = null, System.Data.IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null);
        SqlMapper.GridReader QueryMultiple(string sql, dynamic param = null, System.Data.IDbTransaction transaction = null, int? commandTimeout = null, System.Data.CommandType? commandType = null);
        void RollbackTransaction();

        List<string> GetErros();
    }
}
