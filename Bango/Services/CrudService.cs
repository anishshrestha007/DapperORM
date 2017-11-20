using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bango.Repo;
using Bango.Base.List;
using Bango.Responses;
using System.Reflection;
using Bango.Models;
using System.ComponentModel.DataAnnotations;
using Bango.Helpers;
using Npgsql;
using Bango.Base.Log;
using static Dapper.SqlMapper;
using Bango.Base;
using LightInject;
using Bango.Report;

namespace Bango.Services
{
    public class CrudService<TModel, TKey> : ICrudService<TModel, TKey>
        where TModel : class, Models.IModel, new()
    {
        #region Properties
        public bool CheckClientID { get; set; } = true;
        public bool DisplayMasterDataFromSystem { get; set; } = false;
        /// <summary>
        /// Will be saving all the errors that rose in the service layer
        /// </summary>
        public List<string> Errors { get; set; } = new List<string>();
        /// <summary>
        /// Will store about the errors related to the Unique constraint failure.
        /// </summary>
        public DynamicDictionary ValidationErrors { get; set; } = new DynamicDictionary();
        /// <summary>
        /// The language file that will be used
        /// </summary>
        string LangFileName { get; set; }
        public bool LoadItemAfterSave { get; set; }
        /// <summary>
        /// Sets flag if the chagnes are to be tracked or not. This value is passed to the Repo that performs the CRUD operation.
        /// Default value = true.
        /// </summary>

        public bool TrackChanges { get; set; }
        protected TModel model { get; set; }
        #endregion Properties
        public CrudService()
        {
            model = new TModel();
            model.LoadMetaData();
        }
        protected ModelService _modelService = null;
        public ModelService ModelService
        {
            get
            {
                if (_modelService == null)
                    _modelService = new ModelService();
                return _modelService;
            }
        }
        #region Repos
        protected ICrudRepo<TModel, TKey> _crudRepo = null;
        public ICrudRepo<TModel, TKey> CrudRepo
        {
            get
            {
                if (_crudRepo == null)
                {
                    _crudRepo = InitCrudRepo();
                    _crudRepo.CheckClientID = CheckClientID;
                    _crudRepo.DisplayMasterDataFromSystem = DisplayMasterDataFromSystem;
                    _crudRepo.TrackChanges = TrackChanges;
                }
                return _crudRepo;
            }
            set
            {
                _crudRepo = value;
            }
        }
        public virtual ICrudRepo<TModel, TKey> InitCrudRepo()
        {
            return new CrudRepo<TModel, TKey>();
        }


        protected ISearchRepo<TModel, TKey> _searchRepo = null;
        public ISearchRepo<TModel, TKey> SearchRepo
        {
            get
            {
                if (_searchRepo == null)
                {
                    _searchRepo = InitSearchRepo();
                    _searchRepo.CheckClientID = CheckClientID;
                    _searchRepo.DisplayMasterDataFromSystem = DisplayMasterDataFromSystem;
                }
                return _searchRepo;
            }
            set
            {
                _searchRepo = value;
            }
        }
        public virtual ISearchRepo<TModel, TKey> InitSearchRepo()
        {
            return new SearchRepo<TModel, TKey>();
        }
        #endregion repos

        #region validation
        /// <summary>
        /// Checks if the model being saved is valid or not
        /// </summary>
        /// <param name="item">The model or item that needs to be validated</param>
        /// <returns>Returns the status of the validation</returns>
        public virtual bool IsValid(TModel item)
        {
            return true;
        }

        /// <summary>
        /// Checks if the model being saved is valid or not
        /// </summary>
        /// <param name="item">The item that needs to be validated.
        /// <param name="skipFieldsNotProvided">if only fields which are passed in [item] dictionary has to be validated then set it to true else all fields in model is validated. Default is false.</param>
        /// Note, it first converts the DynamicDictionary to Model before performing validation.</param>
        /// <returns>Returns the status of the validation</returns>
        public virtual bool IsValid(DynamicDictionary item, bool skipFieldsNotProvided = false)
        {
            return IsValid(item, skipFieldsNotProvided, null);
        }

        public virtual bool IsValid(DynamicDictionary item, bool skipFieldsNotProvided = false, DbConnect ConTran = null)
        {
            TModel mdl = new TModel();
            bool model_status = false;
            bool Unique_status = false;
            bool status = false;

            //validating the data being saved
            model_status = this.ModelService.Validate(item, mdl, skipFieldsNotProvided);
            if (!model_status)
            {
                DynamicDictionary ers = new DynamicDictionary();
                //pushing the validation data in the dictionary
                foreach (ValidationResult validation in mdl.ValidationResult)
                {
                    foreach (string member in validation.MemberNames)
                    {
                        ers.Add(member, validation.ErrorMessage);
                    }
                }
                ValidationErrors = ModelService.PushValidationErros(ers, ValidationErrors);
            }


            //now validate for the unique data
            //- validate only those data which are passed for saving

            //using (DbConnect con = ConTran == null ? new DbConnect() : ConTran )
            //{
            //  Unique_status = ValidateUniqueValue(con, item, mdl, skipFieldsNotProvided);
            //}

            DbConnect con = ConTran == null ? new DbConnect() : ConTran;
            Unique_status = ValidateUniqueValue(con, item, mdl, skipFieldsNotProvided);

            if (model_status && Unique_status) //if model data and Unique data both valid then data save else return false...
                status = true;

            //perform other validation
            return status;

        }

        /// <summary>
        /// Performs validation of unique-constraints. Detail on the failure are stored in UniqueErrors Property
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns the status of the validation.</returns>
        public virtual bool ValidateUniqueValue(TModel item)
        {
            return true;
        }

        /// <summary>
        /// Performs validation of unique-constraints. Detail on the failure are stored in UniqueErrors Property
        /// It will use the help of TModel for performing the unique-constraint check.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns the status of the validation.</returns>
        public virtual bool ValidateUniqueValue(DbConnect con, DynamicDictionary item, IModel validatorModel, bool skipFieldsNotProvided = false)
        {
            /**
            TODO
            1) disable null check or check if unique constraint is not composite constraint
            2) check unique constraint in edit mode, load all data
            **/
            PropertyInfo key = validatorModel.GetKeyPropertyInfo();
            TKey id = item.GetValue<TKey>(key.Name);

            //if (Conversion.ToInt32(id.ToString()) > 0)
            //    skipFieldsNotProvided = false; //if edit mode checke

            if (validatorModel.GetType().GetProperty("deleted_uq_code") != null) //tod not equal..
            {
                item["deleted_uq_code"] = 1;
            }
            BangoCommand cmd = new BangoCommand(commandType: MyroCommandTypes.StringBuilder);
            string union = string.Empty;
            //List<PropertyInfo> uniqueFields = new List<PropertyInfo>();
            DictionaryFx<string, PropertyInfo> uniqueFields = new DictionaryFx<string, PropertyInfo>();
            //preparing sql
            DynamicDictionary data_param = null;
            Bango.Models.Attributes.TableDetailAttribute tabelDetail = validatorModel.GetTableDetail();

            foreach (KeyValuePair<string, UniqueConstraint> unique in validatorModel.UniqueFields)
            {
                if (unique.Value.Fields.Count == 0)
                    continue;

                bool value_not_provided = false;

                foreach (string fld in unique.Value.Fields)
                {
                    if (!item.ContainsKey(fld))
                    {
                        //1) disable null check or check if unique constraint is not composite constraint
                        ///if (unique.Value.Fields.Count <= 1)
                        if (unique.Value.Fields.Count <= 1) //TODO:Shivashwor modify... for client duplicate data insert OFF...
                        {
                            value_not_provided = true;
                            break;
                        }

                        if (!skipFieldsNotProvided)
                        {
                            //If fld name not exists in validatorModel then
                            if (validatorModel.GetValue(fld) == null)
                            {
                                ///item.Add(fld, null);
                                value_not_provided = true;
                            }
                            else
                            {
                                Type t = validatorModel.GetType().GetProperty(fld).PropertyType;

                                if (t.IsValueType)
                                {
                                    item.Add(fld, Activator.CreateInstance(t));
                                }
                                else
                                {
                                    item.Add(fld, null);
                                }
                            }
                        }
                        else
                        {
                            //TODO:Shivashwor modify... for client duplicate data insert OFF...
                            value_not_provided = true;
                        }
                        break;
                    }
                }
                if (value_not_provided)
                {
                    continue;
                }

                data_param = (DynamicDictionary)item.Clone();
                ///TODO:SHIVASHWOR 15 nov 2015 for Unique value is empty or not...
                object data_val = data_param.GetValue(unique.Key);
                if (data_val != null)
                {
                    if (data_val.ToString().Trim().Length == 0)
                        continue;
                }

                if (union.Length > 0)
                    cmd.SqlString.AppendLine(union);
                string and = string.Empty;
                cmd.SqlString.AppendLine(String.Format("SELECT distinct '{0}' unique_constraint, '{2}' error_message FROM {1} {3} WHERE 1=1 "
                    , DbServiceUtility.SafeDBString(unique.Value.Name), tabelDetail.Name
                    , DbServiceUtility.SafeDBString(unique.Value.ErrorMessage)
                    , tabelDetail.Alias));
                //CHECKING In if client_id exists in the model for adding the client_id in unique contraint check if the developer has forgot to added
                PropertyInfo prop_client_id = validatorModel.GetType().GetProperty("client_id");
                if (prop_client_id != null)
                {
                    if (!unique.Value.Fields.Contains("client_id"))
                        unique.Value.Fields.Add("client_id");
                }
                foreach (string fld in unique.Value.Fields)
                {
                    if (validatorModel.GetType().GetProperty(fld) != null)
                        DbServiceUtility.BindParameter(cmd, fld, data_param, validatorModel.GetType().GetProperty(fld).PropertyType, tabelDetail.Alias, SearchTypes.Equal | SearchTypes.CaseSensetive, string.Empty, true, validatorModel.GetType().GetProperty(fld));
                    //cmd.SqlString.AppendFormat(" {1} {0} = @{0}", fld, and);//uniqueFields[fld] = validatorModel.GetType().GetProperty(fld);
                }


                if (key.Name.Trim().Length > 0)//PRIMARY KEY Check if n
                {
                    if (id != null)
                    {
                        //var obj_updateBy = data_param.GetValue("updated_by");

                        //if (obj_updateBy!= null)
                        DbServiceUtility.BindParameter(cmd, key.Name, data_param, System.Data.DbType.Int32, tabelDetail.Alias, SearchTypes.NotEqual, string.Empty, true, key);
                    }
                }

                union = " UNION ALL";
            }

            string finalSql = cmd.FinalSql;
            IEnumerable<dynamic> lst = null;
            if (finalSql.Length > 0)
            {
                try
                {
                    lst = con.DB.Query<dynamic>(finalSql, cmd.FinalParameters);
                }
                catch (NpgsqlException ex)
                {
                    Errors.Add(ex.ToString());
                    LogTrace.WriteErrorLog(ex.ToString());
                    LogTrace.WriteDebugLog(string.Format("sql which gave exception:\r{0}", ex.Routine));
                    return false;
                }
                catch (Exception ex)
                {

                }



                //checking for the unique constraint
                if (lst.Count() > 0)
                {
                    foreach (DapperRow dr in lst)
                    {
                        DynamicDictionary dic = Conversion.ToDynamicDictionary(dr);
                        DynamicDictionary err = new DynamicDictionary();
                        err.Add(dic.GetValueAsString("unique_constraint"), dic.GetValue("error_message"));
                        ModelService.PushValidationErros(err, ValidationErrors);
                    }
                    return false;
                }
            }
            else
            {
                //TODO:Shivashwor 01 Nov 2015/
                //if edit mode nothing changed after save data occurs 
                //  throw new NoSqlStringProvidedException();
            }
            return true;
        }

        #endregion validation

        #region Crud operation
        /// <summary>
        /// Inserts the data into the database.
        /// </summary>
        /// <param name="item">Data which need to be inserted</param>
        /// <returns>Returns null if insert fails else returns data in DynamicDictionary</returns>
        public virtual ResponseModel Insert(DynamicDictionary item)
        {
            using (DbConnect con = new DbConnect())
            {

                return Insert(con, item);

            }
        }
        /// <summary>
        /// Inserts the data into the database.
        /// </summary>
        /// <param name="db">The database connection to be used</param>
        /// <param name="item">Data which need to be inserted</param>
        /// <returns>Returns null if insert fails else returns data in DynamicDictionary</returns>
        public virtual ResponseModel Insert(DbConnect con, DynamicDictionary item)
        {
            ResponseModel resp = new ResponseModel(false, string.Empty);
            TModel model = new TModel();
            try
            {
                //validate data before saving
                if (!IsValid((DynamicDictionary)item.Clone(), false, con))
                {
                    resp.message = "Validation failed.";
                    resp.PushValidationErrors(ValidationErrors);
                    return resp;
                }
                //save
                if (CrudRepo.Insert(con, item))
                {
                    if (LoadItemAfterSave)
                    {
                        //PropertyInfo key = Models.ModelService.GetKeyPropertyInfo(item);
                        //TKey id = Models.ModelService.ChangeType<TKey>(key.GetValue(item));
                        //CrudSrvc.get
                        //item = CrudSrvc.GetItemAsModel(id);
                        resp.data = item;
                    }
                    resp.success = true;
                    resp.message = "Data added successfully.";
                }
                else
                {
                    resp.message = "System Error :: DB";
                }
            }
            catch (Exception ex)
            {
                Errors.Add(ex.Message);
            }


            if (resp.success == false)
            {
                resp.PushErrors(CrudRepo.Errors);
                resp.PushErrors(Errors);
                resp.PushValidationErrors(ValidationErrors);
            }
            return resp;
        }

        /// <summary>
        /// Updates the data into the database.
        /// </summary>
        /// <param name="id">The key value of which the data need to be updated.</param>
        /// <param name="item">Returns null if update fails else returns data in DynamicDictionary</returns>
        public virtual ResponseModel Update(TKey id, DynamicDictionary item)
        {
            using (DbConnect con = new DbConnect())
            {
                return Update(con, id, item);
            }
        }
        /// <summary>
        /// Updates the data into the database.
        /// </summary>
        /// <param name="db">The database connection to be used</param>
        /// <param name="id">The key value of which the data need to be updated.</param>
        /// <param name="item">Returns null if update fails else returns data in DynamicDictionary</returns>
        public virtual ResponseModel Update(DbConnect con, TKey id, DynamicDictionary item)
        {
            ResponseModel resp = new ResponseModel(false, string.Empty);
            PropertyInfo key = model.GetKeyPropertyInfo();
            if (key == null)
                throw new Exception("Primary key not defined in business object(model).");

            //validate data before saving
            if (!IsValid(item, true, con))
            {
                resp.message = "Validation failed.";
                resp.PushValidationErrors(ValidationErrors);
                return resp;
            }

            //save
            if (CrudRepo.Update(con, id, item))
            {
                if (LoadItemAfterSave)
                {
                    //PropertyInfo key = Models.ModelService.GetKeyPropertyInfo(item);
                    //TKey id = Models.ModelService.ChangeType<TKey>(key.GetValue(item));
                    //CrudSrvc.get
                    //item = CrudSrvc.GetItemAsModel(id);
                    resp.data = item;
                }
                resp.success = true;
                resp.message = "Data updated successfully.";
            }
            else
            {
                resp.message = "Error while saving data";
            }

            if (resp.success == false)
            {
                resp.PushErrors(CrudRepo.Errors);
                resp.PushErrors(Errors);
                resp.PushValidationErrors(ValidationErrors);
            }
            return resp;
        }

        public virtual ResponseBase Delete(TKey id)
        {
            using (DbConnect con = new DbConnect())
            {
                return Delete(con, id);
            }
        }

        public virtual ResponseBase Delete(DbConnect con, TKey id)
        {
            ResponseBase resp = new ResponseBase(false, string.Empty);
            if (CrudRepo.SoftDelete(con, id))
            {
                resp.success = true;
                resp.message = "Data Deleted successfully.";
            }
            else
            {
                ModelBase mdl = new ModelBase();
                if (CrudRepo.Is_Child_Records_Exists)
                    resp.errors.Add("Parent_child", mdl.GetLang("parent_delete_error"));

                resp.message = "System Error :: DB";
            }

            if (resp.success == false)
            {
                resp.PushErrors(CrudRepo.Errors);
                resp.PushErrors(Errors);
                resp.PushErrors(ValidationErrors);
            }
            return resp;
        }

        public virtual ResponseModel Get(TKey id)
        {
            using (DbConnect con = new DbConnect())
            {
                return Get(con, id);
            }
        }
        public virtual ResponseModel Get(DbConnect con, TKey id)
        {
            ResponseModel resp = new ResponseModel(false, String.Empty);
            DynamicDictionary item = GetAsDictionary(con, id);
            if (item == null)
            {
                resp.message = "Unable to load data.";
                resp.PushErrors(Errors);
            }
            else
            {
                resp.success = true;
                resp.data = item;
                resp.message = "Data loaded successfully.";
            }

            if (resp.success == false)
            {
                resp.PushErrors(CrudRepo.Errors);
                resp.PushErrors(Errors);
            }
            return resp;

        }
        public virtual DynamicDictionary GetAsDictionary(DbConnect con, TKey id)
        {
            DynamicDictionary item = CrudRepo.Get(con, id);
            if (CrudRepo.Errors.Count > 0)
            {
                Errors.AddRange(CrudRepo.Errors);
                return null;
            }
            return item;
        }
        public virtual TModel GetAsModel(DbConnect con, TKey id)
        {
            TModel item = CrudRepo.GetAsModel(con, id);
            if (CrudRepo.Errors.Count > 0)
            {
                Errors.AddRange(CrudRepo.Errors);
                return null;
            }
            return item;
        }

        #endregion Crud operation

        #region Search & List
        /// <summary>
        /// Combo items
        /// </summary>
        /// <param name="data_param"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort_by"></param>
        /// <returns>Returns null if fails else object with or without data.</returns>
        public virtual ResponseCollection GetComboItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null)
        {
            IEnumerable<dynamic> items = null;
            ResponseCollection resp = new ResponseCollection(false, string.Empty);
            using (DbConnect con = new DbConnect())
            {
                items = SearchRepo.GetComboItems(con, data_param, page, pageSize, sort_by);
            }
            if (Errors.Count > 0)
            {
                resp.message = "Combo Data load failed.";
                resp.PushErrors(Errors);
            }
            else
            {
                if (items != null && items.Count() > 0)
                {
                    resp = ServiceUtility.CaculatePaging(resp, items.Count(), page, pageSize);
                    resp.data = items;
                    resp.message = "Combo items loaded successfully.";
                }
                else
                {
                    resp.message = "Combo data not found as per search condition";
                }
                resp.success = true;
            }
            return resp;
        }

        /// <summary>
        /// Grid filter items
        /// </summary>
        /// <param name="data_param"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort_by"></param>
        /// <returns>Returns null if fails else object with or without data.</returns>
        public virtual ResponseCollection GetGridFilterItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null)
        {
            IEnumerable<dynamic> items = null;
            int total = 0;
            DynamicDictionary copy = (DynamicDictionary)data_param.Clone();
            ResponseCollection resp = new ResponseCollection(false, string.Empty);
            using (DbConnect con = new DbConnect())
            {
                //pulling the count;
                total = SearchRepo.GetGridFilterItemsCount(con, copy, page, pageSize);
                if (total > 0)
                {
                    items = SearchRepo.GetGridFilterItems(con, data_param, page, pageSize, sort_by);
                }
            }
            if (Errors.Count > 0)
            {
                resp.message = "Grid filter data load failed.";
                resp.PushErrors(Errors);
            }
            else
            {
                if (total > 0 && items != null && items.Count() > 0)
                {
                    resp = ServiceUtility.CaculatePaging(resp, total, page, pageSize);
                    resp.data = items;
                    resp.message = "Grid filter items loaded successfully.";
                }
                else
                {
                    resp.message = "Grid filter data not found as per search condition";
                }
                resp.success = true;
            }
            return resp;
        }

        /// <summary>
        /// Grid filter items
        /// </summary>
        /// <param name="data_param"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort_by"></param>
        /// <returns>Returns null if fails else object with or without data.</returns>
        public virtual ResponseCollection GetSearchItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null)
        {
            IEnumerable<dynamic> items = null;
            int total = 0;
            DynamicDictionary copy = (DynamicDictionary)data_param.Clone();
            ResponseCollection resp = new ResponseCollection(false, string.Empty);
            using (DbConnect con = new DbConnect())
            {
                //pulling the count;
                total = SearchRepo.GetSearchItemsCount(con, copy, page, pageSize);
                if (total > 0)
                {

                    items = SearchRepo.GetSearchItems(con, data_param, page, pageSize, sort_by);
                }
            }
            if (Errors.Count > 0)
            {
                resp.message = "Grid filter data load failed.";
                resp.PushErrors(Errors);
            }
            else
            {
                resp.success = true;
                if (total > 0 && items != null && items.Count() > 0)
                {
                    resp = ServiceUtility.CaculatePaging(resp, total, page, pageSize);
                    resp.data = items;
                    resp.message = "Search Items loaded successfully.";
                }
                else
                {
                    resp.message = "Grid filter data not found as per search condition";
                }

            }
            return resp;
        }
        #endregion Search & List

        public virtual async Task<IResponseReport> GetListExportPdf(DynamicDictionary data_param, string reportType, string reportName, string sort_by, IReportBase report = null)
        {
            ReponseReport resp = new ReponseReport(false, string.Empty);
            try
            {
                IEnumerable<dynamic> items = null;
                int total = 0;
                DynamicDictionary copy = (DynamicDictionary)data_param.Clone();
                Dictionary<string, object> officeInfo;
                string JsonData, JsonOfficeInfo;

                officeInfo = report.GetOfficeInfo();
                officeInfo.Add("current_user", SessionData.user_name);
                foreach (KeyValuePair<string, object> item in data_param)
                {
                    string k = item.Key;
                    object v = item.Value;
                    officeInfo.Add(k, v);
                }

                using (DbConnect con = new DbConnect())
                {
                    total = SearchRepo.GetSearchItemsCount(con, copy, -1, 1);

                    if (total > 0)
                    {
                        items = SearchRepo.GetSearchItems(con, data_param, -1, 1, sort_by);
                        IList<dynamic> list = (IList<dynamic>)items;
                        if (list == null)
                        {
                            resp.isData = false;
                        }
                    }
                }
                if (Errors.Count > 0)
                {
                    resp.message = "Grid filter data load failed.";
                    resp.PushErrors(Errors);
                }
                else
                {
                    resp.success = true;
                    if (total > 0 && items != null && items.Count() > 0)
                    {
                        JsonData = Newtonsoft.Json.JsonConvert.SerializeObject(items);
                        if (report == null)
                            report = GetReportObject();                      

                        JsonOfficeInfo = Newtonsoft.Json.JsonConvert.SerializeObject(officeInfo);

                        string reportTitle = data_param.GetValueAsString("reportTitle");
                        report.ReportData = "{\"items\":" + JsonData + ",\"info\":" + JsonOfficeInfo + ",\"title\":" + "\"" + reportTitle + "\"" + "}";

                        bool status = await report.GenerateReport(reportType, reportName);
                        if (status)
                        {
                            resp.success = true;
                            resp.report_url = report.GeneratedFileUrl;
                            resp.report_name = report.GeneratedFileName;
                        }
                        else
                        {
                            resp.message = report.Error;
                        }
                    }
                    else
                    {
                        resp.message = "Grid filter data not found as per search condition";
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return resp;
        }

        public virtual IReportBase GetReportObject()
        {
            return App.Container.GetInstance<IReportBase>();
        }
    }
}
//TODO: