using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Bango.Models;
using Bango.Responses;
using Bango.Base.List;
using Bango.Rbac.Roles.AssignedRoles;

namespace ERP.Rbac.Roles.AssignedRoles
{
    [RoutePrefix("api/assignedroles")]
    [Bango.Rbac.RightsPrefix("rbac_role_roles")]
    public class AssignedRolesController : Bango.Controllers.CrudController<AssignedRolesModel, AssignedRolesSearchService, int?>
    {
    }
}