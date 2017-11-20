using Bango.Models.Attributes;
using Bango.Base.List;
using Bango.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Bango.Models
{
    public class ModelBase : IModel//: IModelBase //: ICloneable
    //: IValidatableObject
    {
        [JsonIgnore]
        [NotMapped]
        public string DefaultLangFile { get; set; } = string.Empty;
        [JsonIgnore]
        [NotMapped]
        public List<ValidationResult> ValidationResult { get; set; } = new List<ValidationResult>();
        protected string _sequenceName = null;//"_sequenceName";
        [NotMapped]
        [JsonIgnore]
        public List<string> SelectIgnoreFields = new List<string>() { "created_by", "created_on", "updated_by", "updated_on", "deleted_by", "deleted_on", "is_deleted", "DefaultLangFile", "ValidationResult", "UniqueFields" };
        [NotMapped]
        [JsonIgnore]
        public List<string> DmlIgnoreFields = new List<string>() { "created_by", "created_on", "updated_by", "updated_on", "deleted_by", "deleted_on", "is_deleted", "DefaultLangFile", "ValidationResult", "UniqueFields" };
        private PropertyInfo _keyPropertyinfo;
        [JsonIgnore]
        [NotMapped]
        public UniqueConstraintDictionary UniqueFields { get; set; } = new UniqueConstraintDictionary();
        [NotMapped]
        [JsonIgnore]
        public ChildReferenceList ChildList = new ChildReferenceList();
        protected List<PropertyInfo> _fieldList = new List<PropertyInfo>();
        protected TableDetailAttribute _tableDetail = null;
        protected ComboFieldsAttribute _comboFields = null;
        protected GridFilterFieldsAttribute _gridFilterFields = null;
        public List<PropertyInfo> GetFieldList()
        {
            LoadFields();
            return _fieldList;
        }
        //[JilDirective(Ignore = true)]
        //[NotMapped]
        //public List<PropertyInfo> FieldList = new List<PropertyInfo>();
        public ModelBase()
        {
            // get call stack
            StackTrace stackTrace = new StackTrace();
            StackFrame[] frames = stackTrace.GetFrames();
            bool auto = true;
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                //if (frames[3].GetMethod().Module.Assembly.FullName.Contains("Jil"))
                if (frames[i].GetMethod().Name.Contains("Deserialize"))
                {
                    auto = false;
                    break;
                }
                //if(frames[i].GetMethod().)
            }
            init(auto);
            //SetNull();
        }

        public ModelBase(bool autLoad)
        {
            init(autLoad);
        }


        private void init(bool autLoad)
        {
            initUniqueFields();
            if (autLoad)
                LoadMetaData();
        }
        public virtual void initUniqueFields()
        {
            UniqueFields.Clear();
            UniqueFields.Add("code", GetLang("code") + " " + GetLang("EMPTY_FIELD_ERR_MSG"));
            UniqueFields.Add("name_np", GetLang("name_np") + " " + GetLang("EMPTY_FIELD_ERR_MSG"));
            UniqueFields.Add("name_en", GetLang("name_en") + " " + GetLang("EMPTY_FIELD_ERR_MSG"));
        }
        public virtual void LoadMetaData()
        {
            LoadFields();
            if (_keyPropertyinfo == null)
            {
                _keyPropertyinfo = GetKeyPropertyInfo();
            }

            //Adding the primary key field in dml ignore list
            //DmlIgnoreFields.Add(KeyPropertyName);
        }
        public virtual void LoadFields()
        {
            if (_fieldList.Count == 0)
            {
                PropertyInfo[] props = this.GetType().GetProperties();
                //foreach (PropertyInfo prop in this.GetType().GetProperties())
                for (int i = 0, len = props.Length; i < len; i++)
                {
                    _fieldList.Add(props[i]);
                }
                //_fieldList = this.GetType().GetProperties().ToList<PropertyInfo>();
            }
        }
        public void LoadFromDynamicDictionary(DynamicDictionary item)
        {
            ModelBase model = this;
            PropertyInfo prop;
            List<PropertyInfo> FieldList = model.GetFieldList();
            for (int i = 0, len = FieldList.Count; i < len; i++)
            {
                prop = FieldList[i];
                if (item.ContainsKey(prop.Name.Trim().ToLower()))
                {
                    if (item[prop.Name.Trim().ToLower()] == null)
                    {
                        prop.SetValue(model, null);
                    }
                    else
                    {
                        try
                        {
                            //prop.SetValue(model, ModelService.ChangeType(item[prop.Name.Trim().ToLower()], prop.PropertyType));
                            prop.SetValue(model, ModelService.ChangeType(item.GetValue(prop.Name.Trim().ToLower()), prop.PropertyType));
                        }
                        catch (Exception ex)
                        {
                            prop.SetValue(model, null);
                        }

                    }
                }
            }
        }
        public void LoadFromDataRow(DataRow dr)
        {
            DataColumnCollection columns = dr.Table.Columns;
            ModelBase model = this;
            PropertyInfo prop;
            List<PropertyInfo> FieldList = model.GetFieldList();
            for (int i = 0, len = FieldList.Count; i < len; i++)
            {
                prop = FieldList[i];
                if (dr.Table.Columns.Contains(prop.Name))
                {
                    if (dr[prop.Name].GetType() == typeof(DBNull))
                    {
                        prop.SetValue(model, null);
                    }
                    else
                    {
                        prop.SetValue(model, ModelService.ChangeType(dr[prop.Name], prop.PropertyType));
                        //prop.SetValue(this, Convert.ChangeType(dr[prop.Name], prop.PropertyType));
                    }
                }
            }
        }
        public string GetTableName()
        {
            TableAttribute attrib = this.GetType().GetCustomAttribute<TableAttribute>();
            if (attrib != null)
            {
                return attrib.Name;
            }
            return string.Empty;
        }
        public string GetSequenceName()
        {
            return _sequenceName;
        }
        public string GetAllFields()
        {
            return GetAllFields(string.Empty, false);
        }

        public string GetAllFields(string tableAlias, bool addAlias = true, bool addRowNum = true)
        {
            string append = string.Empty, append2 = string.Empty;
            if (tableAlias.Trim().Length > 0)
            {
                append = tableAlias + ".";
                if (addAlias)
                    append2 = tableAlias + "_";
            }
            StringBuilder sb = new StringBuilder();
            //TODO 
            //if (addRowNum)
            //    sb.AppendFormat("{0} as rnum,", Bango.DB.GetConnection().RowNumKeyword);
            ModelBase model = this;
            for (int i = 0, len = _fieldList.Count; i < len; i++)
            {
                if (_fieldList[i].GetCustomAttribute(typeof(NotMappedAttribute)) != null)
                {
                    continue;
                }
                if (SelectIgnoreFields.Contains(_fieldList[i].Name))
                    continue;
                sb.AppendFormat("{0}{1} as {2}{1},", append, _fieldList[i].Name, append2);
            }
            if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public object GetValue(string field_name)
        {
            object val = null;
            try
            {
                val = this.GetType().GetProperty(field_name).GetValue(this);
            }
            catch (Exception ex)
            {

            }
            return val;

            //object val = null;

            //for (int i = 0, len = FieldList.Count; i < len; i++)
            //{
            //    if (FieldList[i].Name == field_name)
            //    {
            //        val = FieldList[i].GetValue(this);
            //        break;
            //    }
            //    /*if (prop.GetCustomAttribute(typeof(KeyAttribute)) != null)
            //    {
            //        keyField = prop;
            //        break;
            //    }*/
            //}
            //return val;
        }

        public PropertyInfo GetKeyPropertyInfo()
        {
            if (_keyPropertyinfo != null)
                return _keyPropertyinfo;
            foreach (PropertyInfo prop in this.GetType().GetProperties())
            {
                if (prop.GetCustomAttribute(typeof(Models.Attributes.KeyAttribute)) != null)
                {
                    _keyPropertyinfo = prop;
                    break;
                }
            }
            //for (int i = 0, len = _fieldList.Count; i < len; i++)
            //{
            //    if (_fieldList[i].GetCustomAttribute(typeof(KeyAttribute)) != null)
            //    {
            //        keyField = _fieldList[i];
            //        break;
            //    }
            //}
            return _keyPropertyinfo;
        }

        public string GetKeyPropertyName()
        {
            PropertyInfo prop = GetKeyPropertyInfo();
            if (prop == null)
                return string.Empty;
            return prop.Name;
        }

        public TableDetailAttribute GetTableDetail()
        {
            Type typ = this.GetType();
            if (_tableDetail != null)
                return _tableDetail;
            _tableDetail = typ.GetCustomAttribute<TableDetailAttribute>();
            if (_tableDetail == null)
            {
                _tableDetail = new TableDetailAttribute(this.GetType().Name);
            }
            if (_tableDetail.DeleteFlagField?.Length == 0)
            {
                _tableDetail.DeleteFlagField = "is_deleted";
            }
            if (_tableDetail.OrderByField?.Length == 0)
            {
                //string tmp = "name_" + SessionData.language;

                //if(typ.GetProperty("code") != null){
                //    //_tableDetail.OrderByField = "code";
                //    _tableDetail.OrderByField = "id";
                //}
                //else if (typ.GetProperty(tmp) != null)
                //{
                //    _tableDetail.OrderByField = tmp;
                //}
                //else {
                //    _tableDetail.OrderByField = GetKeyPropertyName();
                //}
            }
            return _tableDetail;
        }

        public virtual ComboFieldsAttribute GetComoFields()
        {
            Type typ = this.GetType();
            if (_comboFields != null)
                return _comboFields;
            _comboFields = typ.GetCustomAttribute<ComboFieldsAttribute>();
            if (_comboFields == null)
            {
                _comboFields = new ComboFieldsAttribute();
            }

            if (_comboFields.Id.Trim().Length == 0)
            {
                _comboFields.Id = GetKeyPropertyName();
            }

            if (_comboFields.Code.Trim().Length == 0)
            {
                if (typ.GetProperty("code") == null)
                    _comboFields.Code = _comboFields.Id;
                else
                    _comboFields.Code = "code";
            }

            if (_comboFields.Name.Trim().Length == 0)
            {
                if (typ.GetProperty("name") == null)
                    _comboFields.Name = _comboFields.Id;
                else
                    _comboFields.Name = "name" + SessionData.language;
            }

            if (_comboFields.Text.Trim().Length == 0)
            {
                _comboFields.Text = string.Format("concat_ws(' - ' , {0}, {1}) as text", _comboFields.Code, _comboFields.Name);
            }
            if (_comboFields.OrderBy.Trim().Length == 0)
            {
                _comboFields.OrderBy = _comboFields.Code;
            }
            return _comboFields;
        }

        public GridFilterFieldsAttribute GetGridFilterFields()
        {
            Type typ = this.GetType();
            if (_gridFilterFields != null)
                return _gridFilterFields;
            _gridFilterFields = (GridFilterFieldsAttribute)GetComoFields();
            return _gridFilterFields;
        }

        public T GetChildData<T>()
            where T : class, IModel, new()
        {
            return null;
        }

        public string GetLang(string fileName, string key)
        {
            if (fileName == null)
                return string.Empty;

            if (fileName.Trim().Length == 0)
                return string.Empty;

            return Bango.App.LangMsg.GetLang(fileName, key);
        }


        public string GetLang(string key)
        {
            string res = GetLang(DefaultLangFile, key);

            if (res.Trim().Length == 0)
                res = GetLang("global", key);

            return res;
        }

    }
}
