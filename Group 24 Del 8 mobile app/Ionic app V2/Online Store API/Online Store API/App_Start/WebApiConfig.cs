using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Online_Store_API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            //Code to enable cors
            config.EnableCors(new EnableCorsAttribute("http://localhost:8100", headers: "*", methods: "*"));
            config.MapHttpAttributeRoutes();
           
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


        }
    }
}
