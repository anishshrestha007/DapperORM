using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango;
using Bango.Base.List;
using Bango.Responses;

namespace Bango.Rbac
{
    public class SessionLogService : Services.CrudService<SessionLogModel, int?>
    {
        public SessionLogService()
        {
            CheckClientID = false;
        }
        public override ResponseBase Delete(DbConnect con, int? id)
        {
            ResponseBase resp = new ResponseBase(false, string.Empty);
            if (CrudRepo.HardDelete(con, id))
            {
                resp.success = true;
                resp.message = "Data Deleted successfully.";
            }
            else
            {
                resp.message = "System Error :: DB";
            }

            if (resp.success == false)
            {
                resp.PushErrors(Errors);
                resp.PushErrors(ValidationErrors);
            }
            return resp;
            //return base.Delete(con, id);
        }
    }
}