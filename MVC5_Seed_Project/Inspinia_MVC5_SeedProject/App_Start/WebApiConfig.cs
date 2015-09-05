using System.Web.Http;

class WebApiConfig
{
    public static void Register(HttpConfiguration configuration)
    {
        configuration.MapHttpAttributeRoutes();

        configuration.Routes.MapHttpRoute(
            name: "WithActionApi",
            routeTemplate : "api/{controller}/{action}/{id}",
            defaults: new { id = RouteParameter.Optional}
        );

        configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
    }
}