using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Repo
{
    public class RepoExceptions
    {
    }

    public class DbConnectionNotPassed : Exception
    {
        public override string Message { get; } = "Db Connection is not passed.";
    }

    public class NoSqlStringProvidedException : Exception
    {
        public override string Message { get; } = "Sql string is empty.";
    }


}
