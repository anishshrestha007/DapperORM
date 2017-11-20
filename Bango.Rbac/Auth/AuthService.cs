using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango;
using Bango.Responses;
using Bango.Base.List;
using Bango.Rbac;

namespace Bango.Rbac.Auth
{
    public class AuthService : Bango.Rbac.AuthService
    {
        public new IUserService<Bango.Rbac.User.UserModel, int?> GetUserServiceInstance()
        {
            return new User.UserService();
        }
    }
}