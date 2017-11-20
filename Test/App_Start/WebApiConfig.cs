using Bango;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Test
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Controller Only
            // To handle routes like `/api/VTRouting`
            config.Routes.MapHttpRoute(
                name: "ControllerOnly",
                routeTemplate: "api/{controller}"
            );

            // Controller with ID
            // To handle routes like `/api/VTRouting/1`
            config.Routes.MapHttpRoute(
                name: "ControllerAndId",
                routeTemplate: "api/{controller}/{id}",
                defaults: null,
                constraints: new { id = @"^\d+$" } // Only integers 
            );

            // Controllers with Actions
            // To handle routes like `/api/VTRouting/route`
            config.Routes.MapHttpRoute(
                name: "ControllerAndAction",
                routeTemplate: "api/{controller}/{action}"
            );
            config.Routes.MapHttpRoute(
                name: "file_upload",
                routeTemplate: "api/FileUpload/{action}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional
                }

            );

            App.DisabledControllers.Add("ERP.Tax.Masters.FiscalYear.FiscalYearController");
            App.RegisterWebApiConfig(config);

            var cors = new EnableCorsAttribute("http://localhost:8100/", "*", "*");
            config.EnableCors(cors);
        }
    }
}
