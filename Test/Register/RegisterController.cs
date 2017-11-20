using Bango.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Test.Models.Register;
using Bango.Base.List;
using Bango.Responses;

namespace Test.Controllers.Register
{
    [RoutePrefix("api/Register")]
    public class RegisterController : CrudController<RegisterModel, RegisterService, int?>
    {
        [ActionName("registeruser")]
        public ResponseModel RegisterPost(DynamicDictionary item)
        {
            return base.Post(item);
        }
    }
}