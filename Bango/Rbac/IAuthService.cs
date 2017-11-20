using Bango.Base.List;

namespace Bango.Rbac
{
    public interface IAuthService
    {
        ResponseAuth Authenticate(DynamicDictionary data_param);
        ResponseAuth Authenticate(int client_id, string username, string password);
        DynamicDictionary GetSessionLogDetail(string token, int user_id);
        DynamicDictionary GetSessionLogDetail(DbConnect con, string token, int user_id);
        DynamicDictionary GetTokenDetail(string token, int user_id);
        DynamicDictionary GetTokenDetail(DbConnect con, string token, int user_id);
        IUserService<UserModel, int?> GetUserServiceInstance();
        void Logout();
        DynamicDictionary ValidateToken(string token, int user_id);
        
        DynamicDictionary ValidateToken(DbConnect con, string token, int user_id);
        bool IsValidSession(string token, int user_id);
        bool IsValidSession(DbConnect con, string token, int user_id);
    }
}