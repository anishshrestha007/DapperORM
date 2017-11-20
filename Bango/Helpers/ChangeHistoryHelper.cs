using Dapper;
using Bango.Base;
using Bango.Base.Http;
using Bango.Base.List;
using Bango.Models;
using Bango.Repo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Helpers
{
    
    public class ChangeHistoryHelper<TModel> : Bango.Helpers.IChangeHistoryHelper<TModel>
        where TModel  : Models.IModel, new()
    {
        public LogChangeHistory Changes { get; set; }
        public bool HasChanges { get; set; }
        public AuditActivityTypes ActivityType { get; set; }
        public DynamicParameters Diff { get; set; }
        public dynamic ChangedValues { get; set; }
        public ChangeHistoryHelper(AuditActivityTypes activityType)
        {
            ActivityType = activityType;
            Changes = new LogChangeHistory();
        }
        public List<string> GetIgnoreFields(AuditActivityTypes activityType)
        {
            List<string> lst = new List<string>();
            //lst = new List<string>() { "created_by", "created_on", "updated_by", "updated_on", "deleted_by", "deleted_on", "is_deleted" };
            switch (activityType)
            {
                case AuditActivityTypes.INSERT:
                    
                    lst = new List<string>() { "updated_by", "updated_on", "deleted_by", "deleted_on", "is_deleted" };
                    break;
                case AuditActivityTypes.UPDATE:
                    lst = new List<string>() { "created_by", "created_on", "deleted_by", "deleted_on", "is_deleted" };
                    break;
                case AuditActivityTypes.SOFTDELETE:
                    lst = new List<string>() { "created_by", "created_on", "updated_by", "updated_on" };
                    break;
                default :
                    lst = new List<string>() { "created_by", "created_on", "updated_by", "updated_on", "deleted_by", "deleted_on", "is_deleted" };
                    break;
            }
            return lst;
        }
        public bool CheckChangeChanges(TModel oldData, DynamicDictionary newData)
        {
            HasChanges = false;
            var s = Snapshotter.Start(oldData);
            //start observer
            Diff = new DynamicParameters();
            ChangedValues = new ExpandoObject();
            //merge new data into old data
            ModelService srvc = new ModelService();
            TModel oldDataClone = new TModel();
            srvc.CopyProperty(oldData, oldDataClone);
            List<string> ignoreFields = GetIgnoreFields(ActivityType);
            //bool status = srvc.Merge<TModel>(newData, oldData, GetIgnoreFields(ActivityType));
            //changes
            //Diff = s.Diff();
            List<Dictionary<string, object>> changed_values = new List<Dictionary<string, object>>();
            try
            {
                foreach (PropertyInfo prop in oldData.GetType().GetProperties())
                {

                    if (ignoreFields.Contains<string>(prop.Name))
                    {
                        continue;
                    }
                    if (!newData.ContainsKey(prop.Name))
                    {
                        continue;
                    }
                    string newValue = newData.GetValue(prop.Name) == null ? string.Empty : newData.GetValue(prop.Name).ToString();
                    string oldValue = prop.GetValue(oldData) == null ? string.Empty : prop.GetValue(oldData).ToString();

                    if (newValue != oldValue)
                    {
                        
                        //setting the changed value in property for saving
                        //storing data for saving in change history.
                        HasChanges = true;
                        Dictionary<string, object> row = new Dictionary<string, object>();
                        row.Add("field_name", prop.Name);
                        row.Add("old_value", oldDataClone.GetType().GetProperty(prop.Name).GetValue(oldDataClone));
                        //row.Add("new_value", newData.GetType().GetProperty(prop.Name).GetValue(newData));
                        row.Add("new_value", newData.GetValue(prop.Name));
                        changed_values.Add(row);
                        prop.SetValue(oldData, ModelService.ChangeType(newData.GetValue(prop.Name), prop.PropertyType));

                    }
                }

            }
            catch (Exception ex)
            {
                Bango.Base.Log.LogTrace.WriteErrorLog(ex.ToString());
                throw new Exception("Error while tracking changes.");
            }

            if (HasChanges)
            {
                Changes.table_name = ModelService.GetTableName(oldData);
                Changes.changed_values = JsonConvert.SerializeObject(changed_values);
                Changes.activity_datetime = DateTime.Now;
                Changes.user_id = SessionData.user_id;
                Changes.activity_type = ActivityType.ToString();
                Changes.os_computer_name = HttpRequestHelper.GetClientHostName();
                Changes.os_ipaddress = HttpRequestHelper.GetClientIpAddress();
                Changes.os_useragent = HttpRequestHelper.GetClientBrowser();
                // Changes.pkey_name = Changes.GetKeyPropertyInfo().Name;
            }
            Diff = s.Diff();
            return HasChanges;
        }

        public  dynamic ToDynamic(DynamicDictionary data)
        {
            var eo = new ExpandoObject();
            var eoColl = (ICollection<KeyValuePair<string, object>>)eo;

            foreach (string key in data.KeyList)
            {
                eoColl.Add(new KeyValuePair<string, object>(key, data[key]));
            }

            dynamic eoDynamic = eo;

            return eoDynamic;
        }
        //public bool CheckChangeChanges(TModel oldData, DynamicParameters newData)
        //{
        //    HasChanges = false;
        //    //start observer
        //    Diff = new DynamicParameters();

        //    //merge new data into old data
        //    ModelService srvc = new ModelService();
        //    TModel oldDataClone = new TModel();
        //    srvc.CopyProperty(oldData, oldDataClone);
        //    List<string> ignoreFields = GetIgnoreFields(ActivityType);
        //    //bool status = srvc.Merge<TModel>(newData, oldData, GetIgnoreFields(ActivityType));
        //    //changes
        //    //Diff = s.Diff();
        //    List<Dictionary<string, object>> changed_values = new List<Dictionary<string, object>>();
        //    try
        //    {
        //        foreach (PropertyInfo prop in oldData.GetType().GetProperties())
        //        {

        //            if (ignoreFields.Contains<string>(prop.Name))
        //            {
        //                continue;
        //            }
        //            string newValue = prop.GetValue(newData) == null ? string.Empty : prop.GetValue(newData).ToString();
        //            string oldValue = prop.GetValue(oldData) == null ? string.Empty : prop.GetValue(oldData).ToString();

        //            if (newValue != oldValue)
        //            {
        //                //setting the changed value in property for saving
        //                Diff.Add(prop.Name, prop.GetValue(newData));
        //                //storing data for saving in change history.
        //                HasChanges = true;
        //                Dictionary<string, object> row = new Dictionary<string, object>();
        //                row.Add("field_name", prop.Name);
        //                row.Add("old_value", oldDataClone.GetType().GetProperty(prop.Name).GetValue(oldDataClone));
        //                row.Add("new_value", newData.GetType().GetProperty(prop.Name).GetValue(newData));
        //                changed_values.Add(row);
        //            }
        //        }

        //    }
        //    catch(Exception ex)
        //    {
        //        Bango.Base.Log.LogTrace.WriteErrorLog(ex.ToString());
        //    }

        //    if (HasChanges)
        //    {
        //        Changes.table_name = ModelService.GetTableName(oldData);
        //        Changes.changed_values = JsonConvert.SerializeObject(changed_values);
        //        Changes.activity_datetime = DateTime.Now;
        //        Changes.user_id = Bango.GetCurrentUserId();
        //        Changes.pkey_name = Changes.GetKeyPropertyInfo().Name;
        //    } 
        //    return HasChanges;
        //}
        public bool CheckChangeChanges_old(TModel oldData, TModel newData)
        {
            HasChanges = false;
            //start observer
            var s = Snapshotter.Start(oldData);
            
            //merge new data into old data
            ModelService srvc = new ModelService();
            TModel oldDataClone = new TModel();
            srvc.CopyProperty(oldData, oldDataClone);
            bool status = srvc.Merge<TModel>(newData, oldData, GetIgnoreFields(ActivityType));
            //changes
            Diff = s.Diff();
            List<Dictionary<string, object>> changed_values = new List<Dictionary<string, object>>();
            if (Diff.ParameterNames.AsList<string>().Count > 0)
            {
                foreach (string name in Diff.ParameterNames.AsList<string>())
                {
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    row.Add("field_name", name);
                    row.Add("old_value", oldDataClone.GetType().GetProperty(name).GetValue(oldDataClone));
                    row.Add("new_value", newData.GetType().GetProperty(name).GetValue(newData));
                    //row.Add("new_value", Diff.Get<string>(name));
                    changed_values.Add(row);
                }

                Changes.table_name = ModelService.GetTableName(oldData);
                Changes.changed_values = JsonConvert.SerializeObject(changed_values);
                Changes.activity_datetime = DateTime.Now;
                Changes.user_id = SessionData.user_id;

               // Changes.pkey_name = Changes.GetKeyPropertyInfo().Name;
                Changes.os_computer_name = HttpRequestHelper.GetClientHostName();
                Changes.os_ipaddress = HttpRequestHelper.GetClientIpAddress();
                Changes.os_useragent = HttpRequestHelper.GetClientBrowser();
              //  Changes.mac_address = HttpRequestHelper.GetMacAddress();
                HasChanges = true;
            }
            //if(changes.ParameterNames.le) 
            return HasChanges;
        }

        public bool LogChanges(DbConnect con)
        {
            //return true;
            ChangeHistoryRepo repo = new ChangeHistoryRepo();
            ITable<LogChangeHistory, int?> tbl = con.GetModelTable<LogChangeHistory>();
            try
            {
                //   if(Changes.)
                Bango.Services.ChangeHistoryService _change_history_service= new Bango.Services.ChangeHistoryService();
                DynamicDictionary item = Conversion.ToDynamicDictionary(Changes);
                _change_history_service.Insert(con,item);
            }
            catch(Exception ex)
            {

            }
          
            return true;
        }
        public  string GenerateRequestId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
