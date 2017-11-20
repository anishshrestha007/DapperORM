using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;

namespace Bango.Services
{
    public interface IDbService : IService
    {
        IDatabase Connection { get; set; }
        //IDbTransaction Transaction { get; set; }
        bool BeginTransaction();
        bool CommitTransaction();
        bool RollbackTransaction();
        List<string> Errors { get; set; }
        //IDbService(IDbConnection connection, IDbTransaction transaction);
        //IDbService(IDbConnection connection);

        //IDbService();
    }
}
