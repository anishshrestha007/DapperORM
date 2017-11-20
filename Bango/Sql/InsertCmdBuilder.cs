using Myro.Cmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Myro.Models;
using System.Data;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;
using Myro.Attributes;

namespace Myro.Cmd
{
    public class InsertSqlBuilder<TModel> : SqlBuilderBase<TModel>
        where TModel : ModelBase, new()
    {
        public override IDbCommand BuildCmd()
        {
            _sql.Clear();
            //Command = 
            StringBuilder sb_value = new StringBuilder();
            
            _sql.AppendFormat("INSERT INTO {0} ( ", Model.GetTableName());
            sb_value.Append(" VALUES(");

            IDbCommand cmd = GetCommand();

            PropertyInfo prop = null;
            List<PropertyInfo> FieldList = Model.GetFieldList();
            PropertyInfo keyField = Model.GetKeyPropertyInfo();

            //converting attributes to fields
            for (int i = 0, len = FieldList.Count; i < len; i++)
            {
                prop = FieldList[i];
                if (Model.DmlIgnoreFields.Contains(prop.Name))
                    continue;
                if (FieldList[i].GetCustomAttribute(typeof(NotMappedAttribute)) != null)
                    continue;

                if (prop.Name == keyField.Name)
                {
                    if (Model.GetSequenceName() != null)
                    {
                        //sb.AppendFormat(" {0},", prop.Name);
                        //sb_value.AppendFormat(" {0}.NEXTVAL,", model.GetSequenceName());
                        continue;
                    }
                }
                else
                {

                    if (prop.GetValue(Model) == null)
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
                                _sql.AppendFormat(" {0},", prop.Name);
                                sb_value.AppendFormat(" {0},", App.DB.GetConnection().TimeStampText);
                            }
                            else
                            {
                                _sql.AppendFormat(" {0},", prop.Name);
                                sb_value.AppendFormat(" {0},", prop.Name, App.DB.GetConnection().GetDBDate(prop.GetValue(Model)));
                            }

                        }
                        else
                        {
                            _sql.AppendFormat(" {0},", prop.Name);
                            sb_value.AppendFormat(" {1}{0},", prop.Name, App.DB.GetConnection().ParamPrefix);
                            //sb_value.AppendFormat(" trim({1}{0}),", prop.Name, ParamPrefix);
                            //cmd.Parameters.Add(CreateParameter(prop, Model));
                        }

                    }
                }
            }

            return base.BuildCmd();
        }
    }
}
