using System;
namespace Dapper
{
    public interface ITable<T, TId>
    {
        System.Collections.Generic.IEnumerable<T> All();
        System.Collections.Generic.IEnumerable<T> All(dynamic where = null);
        //bool Delete(dynamic where = null);
        bool Delete(TId id);
        T First(dynamic where = null);
        //T Get(dynamic where);
        T Get(TId id);
        TId Insert(dynamic data);
        TId InsertOrUpdate(dynamic key, dynamic data);
        TId InsertOrUpdate(TId id, dynamic data);
        int InsertWithoutKey(dynamic data);
        //string TableName { get; }
        int Update(dynamic where, dynamic data);
        int Update(TId id, dynamic data);

        System.Collections.Generic.List<string> GetErrors();
    }

    //public interface ITable<T> : ITable<T, int>
    //{
    //}
}
