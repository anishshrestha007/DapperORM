using Bango.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bango.Base.List;

namespace Bango.Repo
{
    public class ChangeHistoryRepo : Repo.CrudRepo<LogChangeHistory, int?>
    {
        public ChangeHistoryRepo()
            : base()
        {
            
            //LoadItemAfterSave = false;
          
        }

    }
}
