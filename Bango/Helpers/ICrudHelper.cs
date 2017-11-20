using System;
namespace Bango.Helpers
{
    public interface ICrudHelper : IDbHelper
    {
        System.Data.IDbCommand GetHardDelete<TModel, TKey>(TKey id) where TModel : Bango.Models.ModelBase, new();
        System.Data.IDbCommand GetInsertCommand(Bango.Models.ModelBase model);
        System.Data.IDbCommand GetItemCommand<TModel, TKey>(TKey id) where TModel : Bango.Models.ModelBase, new();
        System.Data.IDbCommand GetSoftDeleteCommand<TModel, TKey>(TKey id) where TModel : Bango.Models.ModelBase, new();
        System.Data.IDbCommand GetUpdateCommand(Bango.Models.ModelBase model);
    }
}
