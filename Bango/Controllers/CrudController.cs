using Bango.Base.List;
using Bango.Models;
using Bango.Responses;
using Bango.Services;
//using Bango.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.OutputCache.V2;
using LightInject;
using System.Web.Http.Cors;

namespace Bango.Controllers
{
    [AllowCrossSiteJson]
    //[RoutePrefix("api/newcrud")]
    //[Rbac.RightsPrefix("Crud")]
    [EnableCors(origins: "http://localhost:8100", headers: "*", methods: "*")]
    public class CrudController<TModel, TService, TKey> : PrivateController
        where TModel : class, IModel, new()
        where TService : class, ICrudService<TModel, TKey>, new()
    {
        public CrudController()
        {
            //JsReportConfig.ReportUrlBase = Bango.BaseUrl + @"Reports/Outputs";
        }
        [Route("", Order = 9)]
        [Rbac.RightsAuthorize("new")]
        [InvalidateCacheOutput("ComboItems")]
        public virtual ResponseModel Post(DynamicDictionary item)
        {
            TService service = new TService();
            return service.Insert(item);
        }

        [Route("{:id}", Order = 9)]
        [Rbac.RightsAuthorize("edit")]
        [InvalidateCacheOutput("ComboItems")]
        public virtual ResponseBase Put(TKey id, DynamicDictionary item)
        {
            TService service = new TService();
            return service.Update(id, item);
        }

        [Route("{:id}", Order = 9)]
        [Rbac.RightsAuthorize("delete")]
        public virtual ResponseBase Delete(TKey id)
        {
            TService service = new TService();
            return service.Delete(id);
        }

        /// <summary>
        /// Return list of all the countries
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{:id}", Order = 9)]
        [Rbac.RightsAuthorize("view")]
        public virtual ResponseBase Get(TKey id)
        {
            return (new TService()).Get(id);
        }

        [Route("", Order = 9)]
        [Rbac.RightsAuthorize("menu")]
        public virtual ResponseCollection Get(int page = 1, int page_size = 20, string sort_by = null)
        {
            DynamicDictionary paraList = GetQueryAsDictionary();

            //TODO:Shivashwsr 08 jan 2016
            if (paraList.GetValueAsInt("limit") != null)
            {
                int? limit_page = paraList.GetValueAsInt("limit");
                page_size = (int)limit_page;
            }

            return (new TService()).GetSearchItems(paraList, page, page_size, sort_by);
        }

        //[Route("combo/page/{page:int}/page_size/{page_size:int}/sort_by/{sort_by}", Name = "getComboItems")]
        [HttpGet]
        [Rbac.AuthorizeOnly]
        //[CacheOutput(ClientTimeSpan = 2 mintue, ServerTimeSpan = 30 minutes)]
        [CacheOutput(ClientTimeSpan = 120, ServerTimeSpan = 1800)]
        public virtual ResponseCollection ComboItems(string combo, int? page = 0, int? page_size = 0, string sort_by = null)
        {
            DynamicDictionary paraList = GetQueryAsDictionary();
            return (new TService()).GetComboItems(paraList, Convert.ToInt32(page), Convert.ToInt32(page_size), sort_by);
        }

        [HttpGet]
        [Rbac.AuthorizeOnly]
        public virtual ResponseCollection GridFilterItems(string gridfilter, int? page = 0, int? page_size = 0, string sort_by = null)
        {
            DynamicDictionary paraList = GetQueryAsDictionary();
            //TODO:Shivashwsr 08 jan 2016
            if (paraList.GetValueAsInt("limit") != null)
            {
                int? limit_page = paraList.GetValueAsInt("limit");
                page_size = (int)limit_page;
            }

            return (new TService()).GetGridFilterItems(paraList, Convert.ToInt32(page), Convert.ToInt32(page_size), sort_by);
        }

        //[HttpGet]
        //[Rbac.RightsAuthorize("export")]
        //public virtual async Task<IResponseReport> GetListExportPdf(string listExportPdf, string reportType = null, string reportName = null, string sort_by = null)
        //{
        //    DynamicDictionary paraList = GetQueryAsDictionary();
        //    IReportBase report = Bango.Bango.Container.GetInstance<IReportBase>();
        //    return await (new TService()).GetListExportPdf(paraList, reportType, reportName, sort_by, report);
        //    }

    }
}
