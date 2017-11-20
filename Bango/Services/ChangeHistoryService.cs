using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bango.Base.List;
using Bango.Models;
using Bango.Responses;

namespace Bango.Services
{   
     public    class ChangeHistoryService:CrudService<LogChangeHistory,int?>
    {
       public ChangeHistoryService()
        {
            this.TrackChanges = false;
        }
        public override ResponseModel Insert(DbConnect con, DynamicDictionary item)
        {
            
            item.Add("client_id", SessionData.client_id);
            return base.Insert(con, item);
        }
    }
}
