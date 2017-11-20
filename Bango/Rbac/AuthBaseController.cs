using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bango;
using Bango.Base.List;
using Bango.Responses;
using Bango.Controllers;
using System.Web.Http;
using Bango.Models;
using Bango.Services;

namespace Bango.Rbac
{
    public class AuthBaseController<TModel, TService, TKey> : CrudController<TModel, TService, TKey>
        where TModel : class, IModel, new()
        where TService : class, ICrudService<TModel, TKey>, new()
    {
        #region the operation which are cancelled
        public override ResponseBase Put(TKey id, DynamicDictionary item)
        {
            throw new InvalidOperationException("Invalid operation");
            //return base.Put(id, item);
        }
        public override ResponseBase Delete(TKey id)
        {
            throw new InvalidOperationException("Invalid operation");
        }

        public override ResponseCollection ComboItems(string combo, int? page = 0, int? page_size = 0, string sort_by = null)
        {
            throw new InvalidOperationException("Invalid operation");
        }

        public override ResponseCollection Get(int page = 1, int page_size = 20, string sort_by = null)
        {
            throw new InvalidOperationException("Invalid operation");
        }
        public override ResponseBase Get(TKey id)
        {
            throw new InvalidOperationException("Invalid operation");
        }
        public override ResponseCollection GridFilterItems(string gridfilter, int? page = 0, int? page_size = 0, string sort_by = null)
        {
            throw new InvalidOperationException("Invalid operation");
        }

        public override ResponseModel Post(DynamicDictionary item)
        {
            throw new InvalidOperationException("Invalid operation");
        }
        #endregion the operation which are cancelled
        //[HttpPost]
        //[ActionName("auth_authenticate")]
        //public virtual ResponseAuth PostAuthenticate()
        //{
        //    AuthService srvc = new AuthService();

        //    return srvc.Authenticate(GetJosnRequestAsDictionary());
        //}
    }
}