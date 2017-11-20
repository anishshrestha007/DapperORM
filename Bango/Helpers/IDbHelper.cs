using System;
using System.Data;
namespace Bango.Helpers
{
    public interface IDbHelper
    {
        IDbConnection Connection { get; set; }
        //System.Data.IDbDataParameter CreateParameter(System.Reflection.PropertyInfo prop, Bango.Models.ModelBase model);
        System.Data.IDbCommand GetCommand();
        System.Data.IDbCommand GetCommand(string commandText);
        System.Data.IDbDataParameter GetCurrentUserParam(string paramName);
        string ParamPrefix { get; }
        System.Data.IDbTransaction Trans { get; set; }
    }
}
