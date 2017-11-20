using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango;
using Bango.Base.List;
using Bango.Repo;
using Bango.Responses;
using Bango.Base;
using Bango.Models;

namespace Bango.Rbac.Roles
{
    public class RolesService : Bango.Services.CrudService<RolesModel, int?>
    {
        public RolesService()
        {
            DisplayMasterDataFromSystem = true;
        }

        public override ResponseCollection GetComboItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null)
        {
            return base.GetComboItems(data_param, page, pageSize, sort_by);
        }

        public override ISearchRepo<RolesModel, int?> InitSearchRepo()
        {
            //return base.InitSearchRepo();
            return new RoleSearchRepo();
        }

        public override ResponseBase Delete(DbConnect con, int? id)
        {
            ResponseBase resp = new ResponseBase(false, string.Empty);
            if (CrudRepo.SoftDelete(con, id))
            {
                resp.success = true;
                resp.message = "Data Deleted successfully.";
            }
            else
            {
                ModelBase mdl = new ModelBase();
                CrudRepo.Is_Child_Records_Exists = true;
                if (CrudRepo.Is_Child_Records_Exists)
                    resp.errors.Add("Parent_child", mdl.GetLang("parent_delete_error"));

                resp.message = "System Error :: DB";
            }

            if (resp.success == false)
            {
                resp.PushErrors(CrudRepo.Errors);
                resp.PushErrors(Errors);
                resp.PushErrors(ValidationErrors);
            }
            return resp;
        }


    }
}