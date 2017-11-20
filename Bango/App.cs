using LightInject;
using Bango.Base.Log;
using Bango.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Configuration;

namespace Bango
{
    public static class App
    {
        public static FileBox FileBox = new FileBox();
        public static JsonConfig Setting = new JsonConfig();
        public static Config Config = new Config();
        public static OldDbConnect DB { get; set; }
        public static IRegisterType RegisterType { get; private set; }
        public static ServiceContainer Container { get { return RegisterType.Container; } }
        public static Base.List.DynamicDictionary Session { get; set; } = new Base.List.DynamicDictionary();
        public static LangLoader LangMsg = new LangLoader();
        public static List<string> DisabledControllers = new List<string>();

        static App()
        {
            DB = new OldDbConnect();
            FileBox fb = new FileBox();
            LogTraceToFile.DefaultLogFolder = fb.AppPath;
            //Disable following controllers
            DisabledControllers.Add("Bango.Controllers.CrudController");
            DisabledControllers.Add("Bango.Controllers.DiCrudController");
            DisabledControllers.Add("Bango.Controllers.HierarchyController");

            //_baseUrl = String.Concat(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority), HttpContext.Current.Request.ApplicationPath);
            //if (_baseUrl.EndsWith("/"))
            //    _baseUrl = _baseUrl.Substring(0, _baseUrl.Length - 1);
            //if (!_baseUrl.EndsWith("/"))
            //    _baseUrl = string.Concat(_baseUrl, '/');
        }
        
        public static void init(IRegisterType registerType)
        {
            if (FileBox == null)
                FileBox = new FileBox();
            if (Setting == null)
                Setting = new JsonConfig();
            if (Config == null)
                Config = new Config();

            RegisterType = registerType;
            if (RegisterType == null)
                RegisterType = new RegisterTypes();
            //Container = RegisterType.Container;
            //RegisterType.Register();
            

            LangMsg.Load();
        }

        public static void RegisterTypes()
        {

        }
        
        
        public class Target
        {
            public Dictionary<string, object> relation;
        }

        public static Target GetRelationCodes()
        {
            string json = File.ReadAllText(FileBox.JsonFilePath);
            Target newTarget = JsonConvert.DeserializeObject<Target>(json);
            return newTarget;
        }
        public class TargetCat
        {
            public Dictionary<string, object> category;
        }
        public static TargetCat GetCategoryName()
        {
            string json = File.ReadAllText(FileBox.JsonCategoryFilePath);
            TargetCat newTargetCat = JsonConvert.DeserializeObject<TargetCat>(json);
            return newTargetCat;
        }
        public class TargetAuth
        {
            public Dictionary<string, object> type;
        }
        public static TargetAuth GetAuthName()
        {
            string json = File.ReadAllText(FileBox.JsonAuthFilePath);
            TargetAuth newTargetAuth = JsonConvert.DeserializeObject<TargetAuth>(json);
            return newTargetAuth;
        }

        public static string Lang
        {
            get
            {
                return "np";
            }
        }

        public static string DateType
        {
            get
            {
                return Lang == "np" ? "bs" : "ad";
            }
        }

        public static string BaseUrl
        {
            get
            {
                string baseUrl = String.Concat(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority), HttpContext.Current.Request.ApplicationPath);
                if (!baseUrl.EndsWith("/"))
                    baseUrl = string.Concat(baseUrl, '/');
                return baseUrl;
            }
        }
        //public static string BasePath
        //{
        //    get
        //    {

        //    }
        //}
        public static string Version
        {
            get
            {
                if (EnableCache)
                {
                    return WebConfigurationManager.AppSettings["Bango:version"].ToLower().Trim();
                }
                else
                {
                    //return string.Empty;
                    return Convert.ToString(DateTime.Now.ToFileTime());
                }

            }
        }
        public static bool EnableCache
        {
            get
            {
                return WebConfigurationManager.AppSettings["MyroRbac:enablecache"].ToLower().Trim() != "true" ? false : true;
            }
        }
        public static bool CheckToken
        {
            get
            {
                return WebConfigurationManager.AppSettings["MyroRbac:CheckToken"].ToLower().Trim() != "true" ? false : true;
            }
        }
        public static bool CheckRights
        {
            get
            {
                return WebConfigurationManager.AppSettings["MyroRbac:CheckRights"].ToLower().Trim() != "true" ? false : true;
            }
        }

        public static string client_id_field_name
        {
            get { return "client_id"; }
        }

        private static void RegisterWebApiConfig2(HttpConfiguration config)
        {
            // Web API routes
            //config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultSearchApi",
                routeTemplate: "api/{controller}/Search/page/{page}/per_page/{per_page}",
                defaults: new
                {
                    page = RouteParameter.Optional,
                    per_page = RouteParameter.Optional
                }
            );

            //config.Routes.MapHttpRoute(
            //    name: "DefaultSearchApi2",
            //    routeTemplate: "api/{controller}/Search?page={page}&per_page={per_page}",
            //    defaults: new
            //    {
            //        page = RouteParameter.Optional,
            //        per_page = RouteParameter.Optional
            //    }
            //);

            //string query, int page, int per_page, string sort, string order)
            config.Routes.MapHttpRoute(
                name: "childnodes",
                routeTemplate: "api/{controller}/childnodes/parent/{parent_id}",
                defaults: new { parent_id = RouteParameter.Optional }
            );
            
            //config.Routes.MapHttpRoute(
            //    name: "DefaultActionOnlyApi",
            //    routeTemplate: "api/{controller}/{action}"
            //);
            config.Routes.MapHttpRoute(
                name: "DefaultActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
            
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Formatters.JsonFormatter.MediaTypeMappings.Add(new System.Net.Http.Formatting.QueryStringMapping("json", "true", "application/json"));
        }

        public static void RegisterWebApiConfig(HttpConfiguration config)
        {
            // Web API routes

            config.MapHttpAttributeRoutes();
            #region for report
            config.Routes.MapHttpRoute(
                name: "GetListExportPdf",
                routeTemplate: "api/{controller}/ListExportPdf/{reportType}/{reportName}/sort_by",
                defaults: new
                {
                    action = "GetListExportPdf",
                    listExportPdf = "",
                    sort_by = RouteParameter.Optional
                }
            );
            #endregion
            config.Routes.MapHttpRoute(
                name: "auth_authenticate",
                routeTemplate: "api/auth/authenticate",
                defaults: new
                {
                    action = "PostAuthenticate",
                    controller = "Auth"
                }
            );


            //string query, int page, int per_page, string sort, string order)
            config.Routes.MapHttpRoute(
                name: "childnodes",
                routeTemplate: "api/{controller}/childnodes/parent/{parent_id}",
                defaults: new { parent_id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "getComboItemsWithOutPaging",
                routeTemplate: "api/{controller}/combo",
                defaults: new
                {
                    action = "ComboItems",
                    combo = "combo"
                }
            );
            config.Routes.MapHttpRoute(
                name: "getComboItems",
                routeTemplate: "api/{controller}/combo/page/{page}/page_size/{page_size}/sort_by/{sort_by}",
                defaults: new
                {
                    action = "ComboItems",
                    combo = "combo",
                    page = RouteParameter.Optional,
                    page_size = RouteParameter.Optional,
                    sort_by = RouteParameter.Optional
                }
            );
            
            config.Routes.MapHttpRoute(
                name: "getGridFilterItems",
                routeTemplate: "api/{controller}/gridfilter/page/{page}/page_size/{page_size}/sort_by/{sort_by}",
                defaults: new
                {
                    action = "GridFilterItems",
                    page = RouteParameter.Optional,
                    page_size = RouteParameter.Optional,
                    sort_by = RouteParameter.Optional
                }
            );
            config.Routes.MapHttpRoute(
                name: "searchUsingGet",
                routeTemplate: "api/{controller}/page/{page}/page_size/{page_size}/sort_by/{sort_by}",
                defaults: new
                {
                    action = "Get",                    
                    page = RouteParameter.Optional,
                    page_size = RouteParameter.Optional,
                    sort_by = RouteParameter.Optional
                }
            );
            config.Routes.MapHttpRoute(
              name: "ResetPassword",
              routeTemplate: "api/{Controller}/changepassword",
              defaults: new
              {
                  action = "changepassword",
                  Controllers = "User"
              }
          );
            config.Routes.MapHttpRoute(
                name: "Userprofile",
                routeTemplate: "api/{Controller}/changeprofile",
                defaults: new
                {
                    action = "changeprofile",
                    Controllers = "User"
                }
            );
            config.Routes.MapHttpRoute(
                name: "DefaultTreeNodes",
                routeTemplate: "api/{controller}/TreeNodes/StartNode/{start_node_id}",
                defaults: new
                {
                    action = "GetTreeNodes",
                    treenodes = "treenodes",
                    start_node_id = RouteParameter.Optional
                }
            );
            config.Routes.MapHttpRoute(
                name: "DefaultChildNodes",
                routeTemplate: "api/{controller}/ChildNodes/ParentNode/{parent_id}",
                defaults: new
                {
                    action = "GetChildNodes",
                    childnodes = "childnodes",
                    parent_id = RouteParameter.Optional,
                    page = RouteParameter.Optional,
                    page_size = RouteParameter.Optional,
                    sort_by = RouteParameter.Optional
                }
            );
            //config.Routes.MapHttpRoute(
            //    name: "DefaultSearchApi",
            //    routeTemplate: "api/{controller}/search",
            //    //routeTemplate: "api/{controller}/Search/page/{page}/page_size/{page_size}/sort_by/{sort_by}",
            //    defaults: new
            //    {
            //        Action = "PostSearch",
            //        page = RouteParameter.Optional,
            //        page_size = RouteParameter.Optional,
            //        sort_by = RouteParameter.Optional
            //    }
            //);


            //config.Routes.MapHttpRoute(
            //    name: "DefaultCombotItemsApi",
            //    //routeTemplate: "api/{controller}/combo/search?page={page}&page_size={page_size}&sort_by={sort_by}",
            //    routeTemplate: "api/{controller}/comboitems",
            //    defaults: new
            //    {
            //        Action = "comboitems",
            //        combo = RouteParameter.Optional,
            //        page = RouteParameter.Optional,
            //        page_size = RouteParameter.Optional,
            //        sort_by = RouteParameter.Optional
            //    }
            //);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            //config.Routes.MapHttpRoute(
            //    name: "DefaultActionOnlyApi",
            //    routeTemplate: "api/{controller}/{action}",
            //    defaults: new
            //    {
            //        Action = "ActionBased"
            //    }
            //);
            config.Routes.MapHttpRoute(
                name: "DefaultActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Formatters.JsonFormatter.MediaTypeMappings.Add(new System.Net.Http.Formatting.QueryStringMapping("json", "true", "application/json"));
            config.Services.Replace(typeof(System.Web.Http.Dispatcher.IHttpControllerSelector), new MyroHttpControllerSelector(config));

            ////var container = new ServiceContainer();
            //Container.RegisterApiControllers();
            ////register other services
            ////Container.EnablePerWebRequestScope();
            //Container.EnableWebApi(GlobalConfiguration.Configuration);
            ////config.MessageHandlers.Add(new ApiRequestHandler());

            //RegisterType.Register();
        }
    }
}
