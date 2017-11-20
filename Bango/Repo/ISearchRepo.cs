using System.Collections.Generic;
using Bango.Base.List;

namespace Bango.Repo
{
    public interface ISearchRepo<TModel, TKey>
    {
        bool CheckClientID { get; set; }
        bool DisplayMasterDataFromSystem { get; set; }
        List<string> Errors { get; set; }
        BangoCommand AfterBindingOrderBy(SearchScenario searchFor, DbConnect con, BangoCommand cmd, DynamicDictionary data_param, bool count = false, string tableAlias = null);
        BangoCommand AfterBindingParameter(SearchScenario searchFor, DbConnect con, BangoCommand cmd, DynamicDictionary data_param, bool count = false, string tableAlias = null);
        BangoCommand BeforeBindingOrderBy(SearchScenario searchFor, DbConnect con, BangoCommand cmd, DynamicDictionary data_param, bool count = false, string tableAlias = null);
        BangoCommand BeforeBindingParameter(SearchScenario searchFor, DbConnect con, BangoCommand cmd, DynamicDictionary data_param, bool count = false, string tableAlias = null);
        string GetAllFields();
        string GetAllFields(string tableAlias, bool addAlias = true, bool addRowNum = true);
        IEnumerable<dynamic> GetComboItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null);
        IEnumerable<dynamic> GetComboItems(DbConnect con, DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null);
        BangoCommand GetComboItemsCommand(DbConnect con, DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null, bool count = false);
        IEnumerable<dynamic> GetGridFilterItems(DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null);
        IEnumerable<dynamic> GetGridFilterItems(DbConnect con, DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null);
        BangoCommand GetGridFilterItemsCommand(DbConnect con, DynamicDictionary data_param, int page = -1, int pageSize = 20, string sort_by = null, bool count = false);
        int GetGridFilterItemsCount(DynamicDictionary data_param, int page = -1, int pageSize = 20);
        int GetGridFilterItemsCount(DbConnect con, DynamicDictionary data_param, int page = -1, int pageSize = 20);
        BangoCommand GetSearchCommand(SearchScenario scenario, DbConnect con, BangoCommand cmd, DynamicDictionary data_param, string selectClause, string orderByClause, int page = -1, int pageSize = 20, bool count = false, string tableAlias = null, string scenarioOthers = null);
        IEnumerable<dynamic> GetSearchItems(DynamicDictionary data_param, int page, int pageSize, string sort_by = null);
        IEnumerable<dynamic> GetSearchItems(DbConnect con, DynamicDictionary data_param, int page, int pageSize, string sort_by = null);
        BangoCommand GetSearchItemsCommand(DbConnect con, DynamicDictionary data_param, int page, int pageSize, string sort_by = null, bool count = false);
        int GetSearchItemsCount(DynamicDictionary data_param, int page, int pageSize);
        int GetSearchItemsCount(DbConnect con, DynamicDictionary data_param, int page, int pageSize);
    }
}