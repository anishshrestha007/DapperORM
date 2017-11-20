using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dapper.Rainbow
{
    public class SqlDbExpression : Dapper.Rainbow.NpgsqlDbExpression
    {
        //public virtual string IfNull(string column, string value)
        //{
        //    return string.Format(" COALESCE({0}, '{1}')", column, value);
        //}

        //public virtual string IfNull(string column, int value)
        //{
        //    return string.Format(" COALESCE({0}, {1})", column, value);
        //}

        //public virtual string RowNumber(string columnAlias = null)
        //{
        //    if (columnAlias == null)
        //        columnAlias = "rnum";
        //    return string.Format("row_number() OVER () as {0}", columnAlias);
        //}
    }
}
