using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Bango.Models;
using Bango.Responses;
using Bango.Base.List;


namespace Bango.Rbac.User.AssignedUserRoles
{
    [RoutePrefix("api/assigneduserroles")]
    [Bango.Rbac.RightsPrefix("rbac_user_roles")]
    public class AssignedUserRolesController : Bango.Controllers.CrudController<AssignedUserRolesModel, AssignedUserRolesSearchService, int?> //Myro.Controllers.CrudController<assignedrightsModel, assignedrightsRepo, int?>
    {
        [Bango.Rbac.RightsAuthorize("user", "assignrole_new")]
        public override ResponseModel Post(DynamicDictionary item)
        {
            return base.Post(item);
        }
    }
}