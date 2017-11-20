using System.Collections.Generic;
using Bango.Base.List;
using Bango.Models;
using Bango.Repo;
using Bango.Responses;
using System.Threading.Tasks;
using Bango.Report;

namespace Bango.Services
{
    public interface ICrudService<TModel, TKey>
        where TModel : class, IModel, new()
    {
        bool CheckClientID { get; set; }
        ICrudRepo<TModel, TKey> CrudRepo { get; set; }
        ModelService ModelService { get; }
        List<string> Errors { get; set; }
        ISearchRepo<TModel, TKey> SearchRepo { get; set; }
        DynamicDictionary ValidationErrors { get; set; }

        ResponseBase Delete(TKey id);
        ResponseBase Delete(DbConnect con, TKey id);
        ResponseModel Get(TKey id);
        ResponseModel Get(DbConnect con, TKey id);
        DynamicDictionary GetAsDictionary(DbConnect con, TKey id);
        TModel GetAsModel(DbConnect con, TKey id);
        ResponseCollection GetComboItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null);
        ResponseCollection GetGridFilterItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null);
        ResponseCollection GetSearchItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null);

        Task<IResponseReport> GetListExportPdf(DynamicDictionary data_param, string reportType, string reportName, string sort_by = null, IReportBase report = null);

        ICrudRepo<TModel, TKey> InitCrudRepo();
        ISearchRepo<TModel, TKey> InitSearchRepo();
        ResponseModel Insert(DynamicDictionary item);
        ResponseModel Insert(DbConnect con, DynamicDictionary item);
        bool IsValid(TModel item);
        bool IsValid(DynamicDictionary item, bool skipFieldsNotProvided = false);
        ResponseModel Update(TKey id, DynamicDictionary item);
        ResponseModel Update(DbConnect con, TKey id, DynamicDictionary item);
        bool ValidateUniqueValue(TModel item);
        bool ValidateUniqueValue(DbConnect con, DynamicDictionary item, IModel validatorModel, bool skipFieldsNotProvided = false);
    }
}