using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango
{
    class MyroEnums
    {
    }

    public enum Status
    {
        Disable = 0,
        Enable = 1
        
    }

    public enum YesNo
    {
        No = 0,
        Yes = 1
    }

    public enum AuditActivityTypes
    {
        INSERT = 1,
        UPDATE = 10,
        SOFTDELETE = 20,
        HARDDELETE = 25
    }

    [Flags]
    public enum SearchTypes
    {
        Like = 1,
        CaseSensetive = 2,
        Equal = 4,
        StartWith = 8,
        EndWith = 16,
        NotEqual=17,
        isNull=5,
        isNotNull=7,
        IN_Search=9
    }

    public enum MyroCommandTypes
    {
        StringBuilder,
        SqlBuilder,
        StoredProcedure
    }

    public enum BaseDataTypes
    {
        String,
        Number,
        DecimalNumber, //decimal
        DateTime,
        Binary,
        Guid,
        Boolean,
        Other
    }
}
