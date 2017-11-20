using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data;
namespace Bango
{
    public class BangoCommand
    {
        public MyroCommandTypes CommandType { get; set; } = MyroCommandTypes.SqlBuilder;
        public DynamicParameters Parameters { get; set; } = new DynamicParameters();
        public StringBuilder SqlString { get; set; } = new StringBuilder();
        public SqlBuilder SqlBuilder { get; set; } = new SqlBuilder();
        public Dapper.SqlBuilder.Template Template { get; set; }
        
        public BangoCommand()
            : this(new StringBuilder(), new DynamicParameters(), MyroCommandTypes.SqlBuilder)
        {
        }

        private BangoCommand( StringBuilder sql, DynamicParameters parameters, MyroCommandTypes commandType)
        {
            CommandType = commandType;
            SqlString = sql;
            Parameters = parameters;
        }

        static BangoCommand()
        {
            
        }
        public bool SqlStringHasWhere
        {
            get
            {
                string sql = SqlString.ToString().ToLower();
                if (sql.Contains(" where "))
                    return true;
                if (sql.Contains(" where"))
                    return true;
                return false;
            }
        }
        public BangoCommand(StringBuilder sql)
            : this(sql, new DynamicParameters(), MyroCommandTypes.SqlBuilder)
        {
        }
        public BangoCommand(SqlBuilder sqlBuilder)
            : this()
        {
            SqlBuilder = sqlBuilder;
            CommandType = MyroCommandTypes.SqlBuilder;
        }

        public BangoCommand(MyroCommandTypes commandType)
        {
            CommandType = commandType;
            switch (commandType)
            {
                case MyroCommandTypes.SqlBuilder:
                    SqlBuilder = new SqlBuilder();
                    break;
                case MyroCommandTypes.StringBuilder:
                case MyroCommandTypes.StoredProcedure:
                default:
                    SqlString = new StringBuilder();
                    Parameters = new DynamicParameters();
                    break;
            }
        }

        public string FinalSql
        {
            get
            {
                if (CommandType == MyroCommandTypes.SqlBuilder)
                {
                    return Template.RawSql;
                }
                else
                {
                    return SqlString.ToString();
                }
            }
        }

        public DynamicParameters FinalParameters
        {
            get
            {
                if (CommandType == MyroCommandTypes.SqlBuilder)
                {
                    return (DynamicParameters)Template.Parameters;
                }
                else
                {
                    return Parameters;
                }
            }
        }
    }
}
