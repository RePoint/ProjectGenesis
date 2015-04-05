using System.Web.Http;

class WebApiConfig
{
  public static void Register(HttpConfiguration configuration)
  {
    var formatters = GlobalConfiguration.Configuration.Formatters;

    formatters.Remove(formatters.XmlFormatter);

    configuration.Routes.MapHttpRoute("API Default", "api/{controller}/{action}/{id}",
        new { id = RouteParameter.Optional });
  }
}