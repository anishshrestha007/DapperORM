using Bango.Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
namespace Bango.Models
{
    public interface IModel
    {
        string GetAllFields();
        string GetAllFields(string tableAlias, bool addAlias = true, bool addRowNum = true);
        System.Collections.Generic.List<System.Reflection.PropertyInfo> GetFieldList();
        System.Reflection.PropertyInfo GetKeyPropertyInfo();
        string GetKeyPropertyName();
        string GetSequenceName();
        string GetTableName();
        object GetValue(string field_name);
        void LoadFields();
        void LoadFromDataRow(System.Data.DataRow dr);
        void LoadFromDynamicDictionary(Bango.Base.List.DynamicDictionary item);
        void LoadMetaData();
        TableDetailAttribute GetTableDetail();
        ComboFieldsAttribute GetComoFields();
        List<ValidationResult> ValidationResult { get; set; }
        UniqueConstraintDictionary UniqueFields { get; set; }
    }


    
}
