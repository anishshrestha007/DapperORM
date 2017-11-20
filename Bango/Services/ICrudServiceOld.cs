using Bango.Base.List;
using Bango.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Services
{
    public interface ICrudServiceOld<TModel, TKey> : IDbService
        where TModel : IModel
    {
        bool TrackChanges { get; set; }
        bool Insert(DynamicDictionary data);

        //bool Update(TKey id, TModel data);
        bool Update(TKey id, DynamicDictionary data);

        bool SoftDelete(TKey id);

        dynamic Get(TKey id);

        TModel GetItemAsModel(TKey id);
    }
}
