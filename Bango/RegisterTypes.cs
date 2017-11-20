using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LightInject;

namespace Bango
{
    public class RegisterTypes : Helpers.RegisterTypeBase
    {
        public override void Register()
        {
            base.Register();
            //register database (DB)
            Container.Register<System.Data.IDbConnection, Npgsql.NpgsqlConnection>();
            Container.Register<Dapper.Rainbow.IDbExpression, Dapper.Rainbow.NpgsqlDbExpression>();
            Container.Register<Dapper.IDatabase, MyroDatabase>();
            //Container.Register<DB.IDateFormats, DB.DateFormats>();

            
            //helpers
            Container.Register<Helpers.ICrudHelper, Helpers.CrudHelperBase>();
            Container.Register<Helpers.IDbHelper, Helpers.DbHelper>();
            //generics
            Container.Register(typeof(Helpers.IChangeHistoryHelper<>), typeof(Helpers.ChangeHistoryHelper<>));
            //sql or infra helpers

            //rbac


            //response
            Container.Register<Responses.IResponseBase, Responses.ResponseBase>();
            Container.Register<Responses.IResponseModel, Responses.ResponseModel>();
            Container.Register<Responses.IResponseCollection, Responses.ResponseCollection>();

            //RBAC & authentication related
            Container.Register<Rbac.IUserModel, Rbac.UserModel>();
            Container.Register<Rbac.IResponseAuth, Rbac.ResponseAuth>();
            Container.Register(typeof(Rbac.IUserService<,>), typeof(Rbac.UserService<,>));
            Container.Register<Rbac.IAuthService, Rbac.AuthService>();
            //Container.Register(typeof(Rbac.IRightsService<,>), typeof(Rbac.RightsService<,>));
            //Container.Register<Rbac., Rbac.ResponseAuth>();
            //reports related
        }
    }

    public abstract  class DIBase<TRegisterTypes>
        where TRegisterTypes : class , Helpers.IRegisterType, new()
    {
        private static TRegisterTypes _registerType = null;
        public static void init()
        {
            init(new TRegisterTypes());
        }

        public static void init(TRegisterTypes registerType)
        {
            _registerType = registerType;
        }
        public static TRegisterTypes RegisterType
        {
            get
            {
                if (_registerType == null)
                    init();
                return _registerType;
            }
        }
        public static ServiceContainer Container
        {
            get
            {
                return RegisterType.Container;
            }
        }
    }
}
