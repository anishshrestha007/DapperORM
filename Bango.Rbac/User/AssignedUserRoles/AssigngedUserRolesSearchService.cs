using Bango;
using Bango.Repo;
using Bango.Base.List;
using Bango.Responses;
using System.Collections;
namespace Bango.Rbac.User.AssignedUserRoles
{  
    public class AssignedUserRolesSearchService : Bango.Services.CrudService<AssignedUserRolesModel,int?>
    {
        public AssignedUserRolesSearchService()
        {
            DisplayMasterDataFromSystem = true;
        }
        public override ISearchRepo<AssignedUserRolesModel, int?> InitSearchRepo()
        {
            return new AssignedUserRolesRepo();
        }
        public override ResponseModel Insert(DbConnect con, DynamicDictionary item)
        {
            AssignedUserRolesModel model = new AssignedUserRolesModel();
            //setting the client_id before inserting record if client_id field exists.
              if (model.GetType().GetProperty("client_id") != null)
            {
                object roles_user = item.GetValue("assigned_role_ids");
                IEnumerable enumerable = roles_user as IEnumerable;
                if (enumerable != null)
                {
                    foreach (object element in enumerable)
                    {
                        object aa = element;
                        if (item.GetValueAsInt("client_id") == null)
                            item.Add("client_id", SessionData.client_id);
                        else
                            item.Add("client_id", item.GetValueAsInt("client_id"));
                        item.Add("assigned_role_id", aa);
                        base.Insert(con, item);
                    }
                }
                
            }
            ResponseModel Assigned_roles_user = new ResponseModel();
            Assigned_roles_user.success = true;
            Assigned_roles_user.message = "Data Successfully Added.";
            return Assigned_roles_user;
        }
    }
}