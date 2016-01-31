using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using System.Data.Entity;//remove this
using System.Web.Security;

using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Threading;
using Inspinia_MVC5_SeedProject.Models;
using WebMatrix.WebData;

namespace Inspinia_MVC5_SeedProject
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
        }
        protected void Application_BeginRequest()
        
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }
        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
        {
            private static SimpleMembershipInitializer _initializer;
            private static object _initializerLock = new object();
            private static bool _isInitialized;

            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
            }

            private class SimpleMembershipInitializer
            {
                public SimpleMembershipInitializer()
                {
                    try
                    {
                        WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "Id", "UserName", autoCreateTables: true);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Something is wrong", ex);
                    }
                }
            }
        }
    }
}
