using Bango.Base;
using Bango.Base.List;
using Bango.Models;
using Bango.Responses;
using Bango.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//test
namespace Bango.Helpers
{

    public class BatchDataUpdate<TModel, TService, TKey>
        where TModel : class, IModel, new()
        where TService : class, ICrudService<TModel, TKey>, new()
    {
        public delegate ResponseModel SaveCallBackDelegate(DbConnect con, DynamicDictionary item, TKey id);

        public List<string> Errors { get; set; } = new List<string>();
        public string ItemsAddedFieldName { get; set; } = "detail_added";
        public string ItemsModifiedFieldName { get; set; } = "detail_modifed";
        public string ItemsDeletedFieldName { get; set; } = "detail_deleted";
        public string ParentColumnName { get; set; } = "parent_id";
        public TKey ParentColumnValue { get; set; }

        protected TService Service { get; set; } = new TService();

        //Func<DbConnect, DynamicDictionary, int> fun = null)
        public virtual ResponseModel Save(DbConnect con, DynamicDictionary item, TKey id, SaveCallBackDelegate fun = null)
        {

            ResponseModel resp = new ResponseModel();
            resp.success = false;
            Errors.Clear();
            List<DynamicDictionary> items = Conversion.ToDictionaryListFromJson(item.GetValueAsString(ItemsAddedFieldName)); //get Add Data
            resp = BatchInsert(con, items, id, fun);
            if (!resp.success)
            {
                resp.message = "Error while adding records/items.";
                return resp;
            }

            items.Clear();
            items = Conversion.ToDictionaryListFromJson(item.GetValueAsString(ItemsModifiedFieldName)); //get Add Data
            resp = BatchUpdate(con, items, id, fun);
            if (!resp.success)
            {
                resp.message = "Error while updating records/items.";
                return resp;
            }

            items.Clear();
            items = Conversion.ToDictionaryListFromJson(item.GetValueAsString(ItemsDeletedFieldName)); //get Add Data
            resp = BatchDelete(con, items, fun);
            if (!resp.success)
            {
                resp.message = "Error while deleting records/items.";
                return resp;
            }
            resp.success = true;
            resp.message = "Records saved successfully.";
            return resp;
        }

        protected virtual ResponseModel BatchInsert(DbConnect con, List<DynamicDictionary> items, TKey id, SaveCallBackDelegate fun = null)
        {
            ResponseModel resp = new ResponseModel(true, "OK");
            if (items?.Count > 0)
            {
                for (int i = 0, len = items.Count; i < len; i++)
                {
                    //adding parent key before saving.
                    items[i].SetValue(ParentColumnName, ParentColumnValue);
                    resp = Service.Insert(con, items[i]);
                    if (!resp.success)
                    {
                        return resp;
                    }
                    else
                    {
                        if (fun != null)
                        {
                            DynamicDictionary dd = (DynamicDictionary)resp.data;
                            resp = fun(con, items[i], dd.GetValue<TKey>("id"));
                            if (!resp.success)
                                return resp;
                        }

                    }
                }
            }
            return resp;
        }

        protected virtual ResponseModel BatchUpdate(DbConnect con, List<DynamicDictionary> items, TKey id, SaveCallBackDelegate fun = null)
        {
            ResponseModel resp = new ResponseModel(true, "OK");
            if (items?.Count > 0)
            {
                for (int i = 0, len = items.Count; i < len; i++)
                {
                    TModel mdl = new TModel();
                    mdl.LoadFromDynamicDictionary(items[i]);
                    TKey item_id = items[i].GetValue<TKey>(mdl.GetKeyPropertyName());
                    resp = Service.Update(con, item_id, items[i]);
                    if (!resp.success)
                    {
                        return resp;
                    }
                    else
                    {
                        if (fun != null)
                        {
                            DynamicDictionary dd = Service.GetAsDictionary(con, item_id);
                            //DynamicDictionary dd = (DynamicDictionary)resp.data;
                            resp = fun(con, items[i], dd.GetValue<TKey>("id"));
                            if (!resp.success)
                                return resp;
                        }

                    }
                }
            }
            return resp;
        }

        protected virtual ResponseModel BatchDelete(DbConnect con, List<DynamicDictionary> items, SaveCallBackDelegate fun = null)
        {
            ResponseModel resp = new ResponseModel(true, "OK");
            if (items?.Count > 0)
            {
                for (int i = 0, len = items.Count; i < len; i++)
                {
                    TModel mdl = new TModel();
                    mdl.LoadFromDynamicDictionary(items[i]);
                    TKey del_id = items[i].GetValue<TKey>(mdl.GetKeyPropertyName());
                    if (del_id?.ToString().Length > 0)
                    {
                        ResponseBase bs = Service.Delete(con, del_id);
                        resp.errors = bs.errors;
                        resp.success = bs.success;
                        resp.message = bs.message;
                    }
                    if (!resp.success)
                    {
                        return resp;
                    }
                    else
                    {
                        if (fun != null)
                        {
                            //DynamicDictionary dd = Service.GetAsDictionary(con, del_id);
                            DynamicDictionary dd = (DynamicDictionary)resp.data;
                            if (dd == null)//added for voucher detail while deleting the voucher detail grid on editing
                            {
                                resp = fun(con, items[i], del_id);
                            }
                            else
                            {
                                resp = fun(con, items[i], dd.GetValue<TKey>("id"));
                            }
                            if (!resp.success)
                                return resp;
                        }

                    }
                }
            }
            return resp;
        }

        public virtual DynamicDictionary Deserailize(string data)
        {
            return Conversion.ToDictionaryFromJson(data);
        }
        public virtual ResponseBase DeleteChildItems(DbConnect con, string parentColumnName, TKey id)
        {
            DynamicDictionary filter = new DynamicDictionary();
            filter.SetValue(parentColumnName, id);
            filter.SetValue("client_id", SessionData.client_id);
            //TODO : by shakti
            // need to make a function that will only list the less fields for quickly processing.
            IEnumerable<dynamic> tt = Service.SearchRepo.GetSearchItems(filter, -1, 20, null);
            foreach (dynamic i in tt)
            {
                TKey del_id = Conversion.ToDynamicDictionary(i).GetValue<TKey>("id");
                ResponseBase r = Service.Delete(con, del_id);
                if (!r.success)
                    return r;
            }
            return new ResponseBase(true, "Records Deleted Successfully.");
        }

    }
}