using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Bango.Rbac.appManagement.Products
{
    [Bango.Rbac.RightsPrefix("app_products")]
    public class ProductController : Bango.Controllers.CrudController<ProductModel, ProductServices, int?>
    {

    }
}