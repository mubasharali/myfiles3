using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Inspinia_MVC5_SeedProject
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "default1",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            //routes.MapRoute(
            //    name: "some",
            //    url: "ads/{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
            
            //routes.MapRoute(
            //    name: "custom",
            //    url: "{controller}/{category}/{subcategory}/{lowcategory}/{id}/{title}",
            //    defaults: new { controller = "Home", action = "Index", category = UrlParameter.Optional, subcategory = UrlParameter.Optional, lowcategory = UrlParameter.Optional, id = UrlParameter.Optional, title = "" }
            //    );


            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            //below are route suggested by owl88
            //routes.MapRoute(
            //    name: "ID",
            //    url: "{category}/{subcategory}/{lowercategory}/{lowercategory1}/{id}/{ignore}",
            //    defaults: new { controller = "Home", action = "Index", ignore = UrlParameter.Optional }
            //);

            //routes.MapRoute(
            //    name: "LowerCategory1",
            //    url: "{category}/{subcategory}/{lowercategory}/{lowercategory1}",
            //    defaults: new { controller = "Home", action = "Index" }
            //);

            //routes.MapRoute(
            //    name: "LowerCategory",
            //    url: "{category}/{subcategory}/{lowercategory}",
            //    defaults: new { controller = "Home", action = "Index" }
            //);

            //routes.MapRoute(
            //    name: "Subcategory",
            //    url: "{category}/{subcategory}",
            //    defaults: new { controller = "Home", action = "Index" }
            //);

            //routes.MapRoute(
            //    name: "Category",
            //    url: "{category}",
            //    defaults: new { controller = "Home", action = "Index" }
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
