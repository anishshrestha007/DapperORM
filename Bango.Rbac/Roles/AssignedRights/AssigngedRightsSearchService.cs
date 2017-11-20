using Bango;
using Bango.Base.List;
using Bango.Repo;
using Bango.Responses;
using System.Collections;

namespace Bango.Rbac.Roles.AssignedRights
{
    public class assignedrightsSearchService :Bango.Services.CrudService<assignedrightsModel, int?>
    {
        public override ISearchRepo<assignedrightsModel, int?> InitSearchRepo()
        {
            //return base.InitSearchRepo();
            return new assignedrightsRepo();
        }
        public override ResponseModel Insert(DbConnect con, DynamicDictionary item)
        {
            assignedrightsModel model = new assignedrightsModel();
            if (model.GetType().GetProperty("client_id") != null)
            {
                object rights_user = item.GetValue("assigned_right_ids");
                IEnumerable enumerable = rights_user as IEnumerable;
                if (enumerable != null)
                {
                    foreach (object element in enumerable)
                    {
                        object aa = element;
                        if (item.GetValueAsInt("client_id") == null)
                            item.Add("client_id", SessionData.client_id);
                        else
                            item.Add("client_id", item.GetValueAsInt("client_id"));
                        item.Add("assigned_right_id", aa);
                        base.Insert(con, item);
                    }
                }

            }
            ResponseModel Assigned_right_user = new ResponseModel();
            Assigned_right_user.success = true;
            Assigned_right_user.message = "Data Successfully Added.";
            return Assigned_right_user;
        }
    }

}