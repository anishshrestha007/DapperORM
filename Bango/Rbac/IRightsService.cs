using Bango.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Rbac
{
    public interface IRightsService<TModel, TKey> : Services.ICrudService<TModel, TKey>
        where TModel : class, IModel, new()
    {
        bool HasRights(string rightsCode, int user_id);
        Base.List.DynamicDictionary LoadRights(int user_id);
    }
}
