using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Bango;

namespace Bango.Rbac.Roles
{
    [Bango.Rbac.RightsPrefix("rbac_role_master")]
    public class RolesController : Bango.Controllers.CrudController<RolesModel, RolesService, int?>
    {
    }
}