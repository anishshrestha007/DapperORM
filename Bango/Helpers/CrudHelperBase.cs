
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bango.Models;
using System.Reflection;
using Bango.Attributes;
using Bango.Base;
using System.ComponentModel.DataAnnotations.Schema;
namespace Bango.Helpers
{
    public class CrudHelperBase : DbHelper, Bango.Helpers.ICrudHelper, IDbHelper
    {

        public virtual IDbCommand GetUpdateCommand(ModelBase model)
        {
            //preparing the update query
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("UPDATE {0} SET ", model.GetTableName());
            IDbCommand cmd = GetCommand();
            PropertyInfo keyField = model.GetKeyPropertyInfo();
            //:NOTE 
            //THIS has to be done by REPO (its biz logic)
            //PropertyInfo is_verified = model.GetType().GetProperty("is_verified");
            //if (is_verified != null)
            //{
            //    if (Conversion.ToInt32(is_verified.GetValue(model)) == 0)
            //    {
            //        model.GetType().GetProperty("verified_personnel_id").SetValue(model, null);
            //        model.GetType().GetProperty("verified_date_ad").SetValue(model, null);
            //        model.GetType().GetProperty("verified_date_bs").SetValue(model, null);
            //    }
            //}
            PropertyInfo prop = null;
            List<PropertyInfo> FieldList = model.GetFieldList();
            for (int i = 0, len = FieldList.Count; i < len; i++)
            {
                prop = FieldList[i];
                //if the field is in Data manipulation Ignore then don't add it in SQL
                if (model.DmlIgnoreFields.Contains(prop.Name))
                    continue;
                if (FieldList[i].GetCustomAttribute(typeof(NotMappedAttribute)) != null)
                    continue;

                if (prop.GetValue(model) == null)
                {
                    sb.AppendFormat("{0} = NULL,", prop.Name);
                    continue;
                }

                if (prop.Name == keyField.Name)
                {
                    continue;
                }

                if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                {
                    if (prop.GetCustomAttributes(typeof(CurrentTimestampAttribute), true).Length > 0)
                    {
                        //sb.AppendFormat(" {0} = {1},", prop.Name, Connection.TimeStampText);
                    }
                    else
                    {
                        if (prop.GetValue(model).ToString() == "")
                            prop.SetValue(model, null);
                        //sb.AppendFormat(" {0} = {1},", prop.Name, Connection.GetDBDate(prop.GetValue(model)));
                    }
                }
                else
                {
                    //sb.AppendFormat("{0} = trim({1}{0}),", prop.Name, ParamPrefix);
                    sb.AppendFormat("{0} = {1}{0},", prop.Name, ParamPrefix);
                    cmd.Parameters.Add(CreateParameter(prop, model));
                }
                //prop.GetCustomAttributes()

            }
            sb.AppendFormat(" updated_by = {0},", ParamPrefix + "updated_by");
            //sb.AppendFormat(" updated_on = {0}", Connection.TimeStampText);
            cmd.Parameters.Add(GetCurrentUserParam("updated_by"));
            /*if (sb.Length > 0)
                sb.Remove(sb.Length - 1, 1);*/
            //sb.AppendFormat(" WHERE {0} = 0", Connection.IsNull("is_deleted"));
            if (keyField != null)
            {
                sb.AppendFormat(" AND {0} = {1}{0}", keyField.Name, ParamPrefix);
                cmd.Parameters.Add(CreateParameter(keyField, model));
            }
            sb.AppendFormat(" RETURNING {0}", model.GetAllFields(string.Empty, false, false));
            cmd.CommandText = sb.ToString();
            return cmd;
        }

        public virtual IDbCommand GetSoftDeleteCommand<TModel, TKey>(TKey id)
            where TModel : ModelBase, new()
        {
            TModel model = new TModel();

            //preparing the update query
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("UPDATE {0} SET ", model.GetTableName());
            IDbCommand cmd = GetCommand();
            PropertyInfo keyField = model.GetKeyPropertyInfo();

            //set the delete fields value
            sb.AppendFormat(" deleted_by = {0}deleted_by,", ParamPrefix);
            //sb.AppendFormat(" deleted_on = {0},", Connection.TimeStampText);
            sb.Append(" is_deleted = 1");
            cmd.Parameters.Add(GetCurrentUserParam("deleted_by"));

            sb.Append(" WHERE NVL(is_deleted, 0) = 0");
            if (keyField != null)
            {
                keyField.SetValue(model, id);
                sb.AppendFormat(" AND {0} = {1}{0}", keyField.Name, ParamPrefix);
                cmd.Parameters.Add(CreateParameter(keyField, model));
            }

            cmd.CommandText = sb.ToString();
            return cmd;
        }

        public virtual IDbCommand GetHardDelete<TModel, TKey>(TKey id)
             where TModel : ModelBase, new()
        {
            TModel model = new TModel();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Delete from {0}", model.GetTableName());
            IDbCommand cmd = GetCommand();
            PropertyInfo keyField = model.GetKeyPropertyInfo();
            if (keyField != null)
            {
                keyField.SetValue(model, id);
                sb.AppendFormat(" where {0} = {1}{0}", keyField.Name, ParamPrefix);
                cmd.Parameters.Add(CreateParameter(keyField, model));
            }
            cmd.CommandText = sb.ToString();
            return cmd;
        }

        public virtual IDbCommand GetInsertCommand(ModelBase model)
        {
            //preparing the update query
            StringBuilder sb = new StringBuilder(),
                sb_value = new StringBuilder();

            sb.AppendFormat("INSERT INTO {0} ( ", model.GetTableName());
            sb_value.Append(" VALUES(");

            IDbCommand cmd = GetCommand();
            
            PropertyInfo prop = null;
            List<PropertyInfo> FieldList = model.GetFieldList();
            PropertyInfo keyField = model.GetKeyPropertyInfo();
            for (int i = 0, len = FieldList.Count; i < len; i++)
            {
                prop = FieldList[i];
                if (model.DmlIgnoreFields.Contains(prop.Name))
                    continue;
                if (FieldList[i].GetCustomAttribute(typeof(NotMappedAttribute)) != null)
                    continue;

                if (prop.Name == keyField.Name)
                {
                    if (model.GetSequenceName() != null)
                    {
                        //sb.AppendFormat(" {0},", prop.Name);
                        //sb_value.AppendFormat(" {0}.NEXTVAL,", model.GetSequenceName());
                        continue;
                    }
                }
                else
                {

                    if (prop.GetValue(model) == null)
                    {
                        //db default will be saved.
                        continue;
                    }
                    else
                    {

                        //if (prop.Name == "date_ad" || prop.Name == "date_bs" || prop.Name == "dob_ad")
                        if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                        {
                            if (prop.GetCustomAttributes(typeof(CurrentTimestampAttribute), true).Length > 0)
                            {
                                sb.AppendFormat(" {0},", prop.Name);
                                //sb_value.AppendFormat(" {0},", Connection.TimeStampText);
                            }
                            else
                            {
                                sb.AppendFormat(" {0},", prop.Name);
                                //sb_value.AppendFormat(" {0},", prop.Name, Connection.GetDBDate(prop.GetValue(model)));
                            }

                        }
                        else
                        {
                            sb.AppendFormat(" {0},", prop.Name);
                            sb_value.AppendFormat(" {1}{0},", prop.Name, ParamPrefix);
                            //sb_value.AppendFormat(" trim({1}{0}),", prop.Name, ParamPrefix);
                            cmd.Parameters.Add(CreateParameter(prop, model));
                        }

                    }
                }
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
                sb.Append(")");
            }
            if (sb_value.Length > 0)
            {
                sb_value.Remove(sb_value.Length - 1, 1);
                sb_value.Append(") ");
                sb.Append(sb_value.ToString());
            }

            //TODO: this code is Postgre specific so need to be changed in future to make way for other database
            // This line return the primary key value which is recently added
            sb.AppendFormat(" RETURNING {0}", model.GetAllFields(string.Empty, false, false));
            cmd.CommandText = sb.ToString();
            return cmd;
        }

        public virtual IDbCommand GetItemCommand<TModel, TKey>(TKey id)
            where TModel : ModelBase, new()
        {
            TModel model = new TModel();
            //preparing the update query
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT {0}", model.GetAllFields());
            sb.AppendFormat(" FROM {0} ", model.GetTableName());

            //sb.AppendFormat(" WHERE {0} = 0", Connection.IsNull("is_deleted"));

            IDbCommand cmd = GetCommand();

            PropertyInfo prop = model.GetKeyPropertyInfo();
            //BindParameters(cmd, model, new Dictionary<string, object>().Add(prop.Name, id));
            sb.AppendFormat(" AND {0} = {1}{0}", prop.Name, ParamPrefix);
            prop.SetValue(model, id);
            cmd.Parameters.Add(CreateParameter(prop, model));
            sb.AppendFormat(" ORDER BY {0}", prop.Name);
            cmd.CommandText = sb.ToString();
            return cmd;
        }

        public virtual TKey GetRecentInsertedId<TModel, TKey>(TModel model)
        {
            //Errors.Clear();
            //StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("SELECT {0}.CURRVAL FROM DUAL", model.GetSequenceName());
            TKey id;
            //using (DbConnectionBase con = Db.GetOracleConnection())
            //{
            //    id = ModelBase.ChangeType<TKey>(con.ExecuteScalar(sb));
            //    if (con.Error.Length > 0)
            //    {
            //        Errors.Add(con.Error);
            //    }
            //}
            return default(TKey);
        }

        IDbConnection IDbHelper.Connection
        {
            get
            {
                return base.Connection;
            }
            set
            {
                base.Connection = value;
            }
        }


        IDbCommand IDbHelper.GetCommand()
        {
            return base.GetCommand();
        }

        IDbCommand IDbHelper.GetCommand(string commandText)
        {
            return base.GetCommand(commandText);
        }

        IDbDataParameter IDbHelper.GetCurrentUserParam(string paramName)
        {
            return base.GetCurrentUserParam(paramName);
        }

        string IDbHelper.ParamPrefix
        {
            get { return base.ParamPrefix; }
        }

        IDbTransaction IDbHelper.Trans
        {
            get
            {
                return base.Trans;
            }
            set
            {
                base.Trans = value;
            }
        }
    }
}
