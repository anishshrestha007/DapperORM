using Bango.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Rbac
{
    public interface IUserService<TModel, TKey> : Services.ICrudService<TModel, TKey>
        where TModel : class, IUserModel, new()
    {
        ResponseAuth AuthenticateUserNamePasword(DbConnect con, int client_id, string username, string password);
        string EncryptPassword(string password);
        string DecryptPassword(string password);
        bool HasRights(string rightsCode, int user_id);
        bool HasRights(string rightsCode);
        Base.List.DynamicDictionary LoadRights(int user_id);
        Base.List.DynamicDictionary LoadRoles(int user_id);
        //bool HasRights(string rightsCode, int user_id);
        //Base.List.DynamicDictionary LoadRights(int user_id);
    }
}
