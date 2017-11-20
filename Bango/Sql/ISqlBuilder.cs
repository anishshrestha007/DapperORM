using Myro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Myro.Cmd
{
    public interface ICmdBuilder
    {
        //QueryTypes QueryType;
        //bool CreateParameterizedCommand;
        IDbCommand GetCmd();
        IDbCommand GetCmd(ModelBase model);
        //StringBuilder BuildQuery(QueryTypes queryType);
        //StringBuilder BuildQuery(bool createParameterizedCommand);
        //StringBuilder BuildQuery(QueryTypes queryType, bool createParameterizedCommand);
    }

    public enum SqlTypes
    {
        Select = 0,
        Insert,
        Update,
        Delete
    }

    public enum ClauseTypes
    {
        Column = 0,
        InsertValues,
        UpdateValues,
        From,
        Where,
        OrderBy,
        GroupBy,
        Having
    }
}
