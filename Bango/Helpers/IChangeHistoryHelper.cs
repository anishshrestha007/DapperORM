using Bango.Base.List;
using System;
namespace Bango.Helpers
{
    public interface IChangeHistoryHelper<TModel>
    {
        AuditActivityTypes ActivityType { get; set; }
        Bango.Models.LogChangeHistory Changes { get; set; }
        //bool CheckChangeChanges(TModel oldData, TModel newData);
        bool CheckChangeChanges(TModel oldData, DynamicDictionary newData);
        bool HasChanges { get; set; }
        bool LogChanges(DbConnect con);
    }
}
