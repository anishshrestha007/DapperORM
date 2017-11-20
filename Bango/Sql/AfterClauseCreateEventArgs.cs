using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myro.Cmd
{
    public class BeforeClauseCreateEventArgs : CancelEventArgs
    {
        public SqlTypes SqlType { get; set; }
        public ClauseTypes ClauseType { get; set; }
        public StringBuilder SqlClause { get; set; }
        public BeforeClauseCreateEventArgs(SqlTypes queryType, StringBuilder clause)
        {
            SqlType = queryType;
            SqlClause = clause;
        }
            
        public BeforeClauseCreateEventArgs(SqlTypes queryType, ClauseTypes clauseType, StringBuilder clause)
        {
            SqlType = queryType;
            ClauseType = clauseType;
            SqlClause = clause;
        }
    }
    public delegate void BeforeClauseCreateEventHandler(object sender, BeforeClauseCreateEventArgs e);
    public class AfterClauseCreateEventArgs : EventArgs
    {
        public ClauseTypes ClauseType { get; set; }
        public AfterClauseCreateEventArgs(SqlTypes sqlType, ClauseTypes clauseType, StringBuilder clause)
        {
            SqlType = sqlType;
            ClauseType = clauseType;
            SqlClause = clause;
        }
        public AfterClauseCreateEventArgs(SqlTypes sqlType, StringBuilder clause)
        {
            SqlType = sqlType;
            SqlClause = clause;
        }

        public SqlTypes SqlType { get; set; }

        public StringBuilder SqlClause { get; set; }
     
    }
    public delegate void AfterClauseCreateEventHandler(object sender, AfterClauseCreateEventArgs e);
    


    public delegate void AfterSqlCreateEventHandler(object sender, AfterSqlCreateEventArgs  e);
    public class AfterSqlCreateEventArgs : EventArgs
    {
        public SqlTypes QueryType { get; set; }

        public StringBuilder Sql { get; set; }

        public AfterSqlCreateEventArgs(SqlTypes queryType, StringBuilder sql)
        {
            QueryType = queryType;
            Sql = sql;

        }


    }

    public class BeforeSqlCreateEventArgs : CancelEventArgs
    {
        public SqlTypes QueryType { get; set; }
        public StringBuilder Sql { get; set; }
        public BeforeSqlCreateEventArgs(SqlTypes queryType, StringBuilder sql)
        {
            QueryType = queryType;
            Sql = sql;
        }
    }
    public delegate void BeforeSqlCreateEventHandler(object sender, BeforeSqlCreateEventArgs e);

    
}
