using System;
namespace Dapper
{
    public interface ISqlBuilder
    {
        SqlBuilder AddParameters(dynamic parameters);
        SqlBuilder.Template AddTemplate(string sql, dynamic parameters = null);
        SqlBuilder GroupBy(string sql, dynamic parameters = null);
        SqlBuilder Having(string sql, dynamic parameters = null);
        SqlBuilder InnerJoin(string sql, dynamic parameters = null);
        SqlBuilder Join(string sql, dynamic parameters = null);
        SqlBuilder LeftJoin(string sql, dynamic parameters = null);
        SqlBuilder OrderBy(string sql, dynamic parameters = null);
        SqlBuilder RightJoin(string sql, dynamic parameters = null);
        SqlBuilder Select(string sql, dynamic parameters = null);
        SqlBuilder Where(string sql, dynamic parameters = null);
    }
}
