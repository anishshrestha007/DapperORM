using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Bango.Models;
using Bango.Responses;
using Bango.Base.List;

namespace Bango.Rbac.User.AssignedUserRights
{
    [RoutePrefix("api/assigneduserrights")]
    [Bango.Rbac.RightsPrefix("rbac_user_rights")]
    public class AssignedUserRightsController : Bango.Controllers.CrudController<AssignedUserRightsModel, AssignedUserRightsSearchService, int?> //Myro.Controllers.CrudController<assignedrightsModel, assignedrightsRepo, int?>
    {
    }
}