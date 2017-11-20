using Bango.Base;
using Bango.Base.List;
using Bango.Models.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Bango.Models
{
    public class ModelService
    {
        public static DynamicDictionary ToDictionary(IModel model, bool AddIgnoredItems = false)
        {
            model.LoadMetaData();
            DynamicDictionary dic = new DynamicDictionary();
            List<PropertyInfo> FieldList = model.GetFieldList();
            for (int i = 0, len = FieldList.Count; i < len; i++)
            {
                if (AddIgnoredItems == false)
                {
                    if (FieldList[i].GetCustomAttribute<JsonIgnoreAttribute>() != null)
                        continue;
                }
                dic.Add(FieldList[i].Name, FieldList[i].GetValue(model));
            }
            return dic;
        }

        public virtual bool Merge<TModel>(TModel source, TModel destination)
            where TModel : IModel
        {
            return Merge(source, destination, new List<string>());
        }

        public virtual bool Merge<TModel>(TModel source, TModel destination, List<string> ignoreFields)
            where TModel : IModel
        {
            source.LoadMetaData();
            destination.LoadMetaData();
            foreach (PropertyInfo prop in destination.GetType().GetProperties())
            {
                if (ignoreFields.Contains(prop.Name))
                {
                    continue;
                }
                prop.SetValue(destination, prop.GetValue(source));
            }
            return true;
        }

        public virtual bool TrackChanges<TModel>(TModel source, TModel destination, List<string> ignoreFields)
           where TModel : IModel
        {
            source.LoadMetaData();
            destination.LoadMetaData();
            foreach (PropertyInfo prop in destination.GetType().GetProperties())
            {
                if (ignoreFields.Contains(prop.Name))
                {
                    continue;
                }
                prop.SetValue(destination, prop.GetValue(source));
            }
            return true;
        }
        //public virtual bool CopyProperty(ModelBase sourceModel, ModelBase destination)
        public virtual bool CopyProperty(IModel sourceModel, IModel destination)
        {
            sourceModel.LoadMetaData();
            destination.LoadMetaData();

            foreach (PropertyInfo prop in destination.GetType().GetProperties())
            {
                prop.SetValue(destination, prop.GetValue(sourceModel));
            }
            return true;
            //throw new NotImplementedException();
        }

        public virtual void SetNull(ModelBase model)
        {
            model.LoadMetaData();
            List<PropertyInfo> FieldList = model.GetFieldList();
            for (int i = 0, len = FieldList.Count; i < len; i++)
            {
                if (FieldList[i].GetCustomAttribute(typeof(Models.Attributes.RequiredAttribute)) == null)
                {
                    FieldList[i].SetValue(model, null);
                }
            }
        }

        /// <summary>
        /// Performs validation on the data passed in item based on the model passed in TModel. The developer can use the third parameter ony fields passed need to be validated.
        /// </summary>
        /// <typeparam name="TModel">The model based on which the data validation is performed.</typeparam>
        /// <param name="item">Dictionary with data which needed to be validated.</param>
        /// <param name="skipFieldsNotProvided">if only fields which are passed in [item] dictionary has to be validated then set it to true else all fields in model is validated. Default is false.</param>
        /// <returns>Returns true if validation is ok else false</returns>
        public virtual bool Validate<TModel>(DynamicDictionary item, bool skipFieldsNotProvided = false)
            where TModel : IModel, new()
        {
            TModel model = new TModel();
            return Validate(item, model);
        }
        /// <summary>
        /// Performs validation on the data passed in item based on the model passed in validatorModel. The developer can use the third parameter ony fields passed need to be validated.
        /// </summary>
        /// <param name="item">Dictionary with data which needed to be validated.</param>
        /// <param name="validatorModel">The model based on which the data validation is performed.</param>
        /// <param name="skipFieldsNotProvided">if only fields which are passed in [item] dictionary has to be validated then set it to true else all fields in model is validated. Default is false.</param>
        /// <returns>Returns true if validation is ok else false</returns>
        public virtual bool Validate(DynamicDictionary item, IModel validatorModel, bool skipFieldsNotProvided = false)
        {
            validatorModel.LoadFromDynamicDictionary(item);
            var context = new ValidationContext(validatorModel, serviceProvider: null, items: null);
            validatorModel.ValidationResult = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(validatorModel, context, validatorModel.ValidationResult, validateAllProperties: true);
            if (!isValid)
            {
                if (skipFieldsNotProvided)
                {
                    for (int vCnt = validatorModel.ValidationResult.Count - 1; vCnt >= 0; vCnt--)
                    {
                        List<string> members = validatorModel.ValidationResult[vCnt].MemberNames.ToList();
                        for (int mCnt = members.Count - 1; mCnt >= 0; mCnt--)
                        {
                            if (!item.ContainsKey(members[mCnt].Trim().ToLower()))
                            {
                                members.RemoveAt(mCnt);
                            }
                        }
                        if (members.Count == 0)
                        {
                            validatorModel.ValidationResult.RemoveAt(vCnt);
                        }
                    }
                    if (validatorModel.ValidationResult.Count == 0)
                        isValid = true;
                }

            }
            return isValid;
        }

        public virtual string GetFilteredFields(ModelBase model)
        {
            model.LoadMetaData();
            StringBuilder sb = new StringBuilder();
            List<PropertyInfo> FieldList = model.GetFieldList();
            for (int i = 0, len = FieldList.Count; i < len; i++)
            {
                if (model.SelectIgnoreFields.Contains(FieldList[i].Name))
                    continue;
                sb.Append(FieldList[i].Name + ",");
            }
            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public static T ChangeType<T>(object value)
        {
            return Base.TypeHelper.ChangeType<T>(value);
        }

        public static object ChangeType(object value, Type conversion)
        {
            return Base.TypeHelper.ChangeType(value, conversion);
        }
        public static string GetTableName(Type modelType)
        {
            TableAttribute attrib = modelType.GetCustomAttribute<TableAttribute>();
            if (attrib != null)
            {
                return attrib.Name;
            }
            return string.Empty;
        }
        public static string GetTableName(IModel model)
        {
            TableAttribute attrib = model.GetType().GetCustomAttribute<TableAttribute>();
            if (attrib != null)
            {
                return attrib.Name;
            }
            return string.Empty;
        }

        public static PropertyInfo GetKeyPropertyInfo(IModel model)
        {
            foreach (PropertyInfo prop in model.GetType().GetProperties())
            {
                if (prop.GetCustomAttribute(typeof(Models.Attributes.KeyAttribute)) != null)
                {
                    return prop;
                }
            }
            return null;
        }

        public static PropertyInfo GetDeleteFieldProperty(IModel model)
        {
            TableDetailAttribute attrib = model.GetTableDetail();
            if (attrib.DeleteFlagField.Trim().Length > 0) {
                return model.GetType().GetProperty(attrib.DeleteFlagField);
            }
            return null;
        }
        public static string GetDeleteFieldName(IModel model)
        {
            TableDetailAttribute attrib = model.GetTableDetail();
            return attrib.DeleteFlagField;
        }

        public string GetKeyPropertyName(IModel model)
        {
            PropertyInfo prop = GetKeyPropertyInfo(model);
            if (prop == null)
                return string.Empty;
            return prop.Name;
        }

        public static DynamicDictionary PushValidationErros(DynamicDictionary soruce, DynamicDictionary destination)
        {
            foreach (string s in soruce.KeyList)
            {
                if (destination.ContainsKey(s))
                {
                    List<string> msgs = new List<string>();
                    if (destination.GetValue(s).GetType() == typeof(List<string>))
                    {
                        msgs = (List<string>)destination.GetValue(s); //soruce
                    }
                    else
                    {
                        msgs.Add(destination.GetValue(s).ToString());
                    }

                    if (soruce.GetValue(s).GetType() == typeof(List<string>))
                    {
                        List<string> n = (List<string>)soruce.GetValue(s);
                        msgs.AddRange(n);
                    }
                    else
                    {
                        msgs.Add(soruce.GetValue(s).ToString());

                    }
                    destination.SetValue(s, msgs);
                }
                else
                {
                    destination.Add(s, soruce.GetValue(s));
                }
            }
            return destination;
        }

        public static List<TModel> ToList<TModel>(IEnumerable<dynamic> items)
            where TModel : class, Models.IModel, new()
        {
            List<TModel> lst = new List<TModel>();

            if (items == null)
                return lst;

            foreach(DapperRow itm in items)
            {
                lst.Add(ToModel<TModel>(itm));
            }
            return lst;
        }

        public static TModel ToModel<TModel>(dynamic item)
            where TModel : class, Models.IModel, new()
        {
            DynamicDictionary itemdd = Conversion.ToDynamicDictionary(item);
            TModel mdl = new TModel();
            mdl.LoadFromDynamicDictionary(itemdd);
            return mdl;
        }

        /// <summary>
        ///  objec to model conversion..
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static TModel ToModel_Dapper_Row<TModel>(IEnumerable<dynamic> items)
    where TModel : class, Models.IModel, new()
        {
            List<TModel> lst = new List<TModel>();
            TModel mdl = new TModel();

            if (items == null)
                return null;

            foreach (DapperRow itm in items)
            {
                lst.Add(ToModel<TModel>(itm));
            }

            if (lst.Count == 0)
                return null;

            mdl = ToModel<TModel>(lst[0]);

            return mdl;
        }
    }
}
