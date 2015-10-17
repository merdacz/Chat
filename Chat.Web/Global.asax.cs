namespace Chat.Web
{
    using System.Web.Mvc;
    using System.Web.Routing;

    using log4net.Config;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalFilters.Filters.Add(new ExceptionHandlingAttribute());
            XmlConfigurator.Configure();
        }
    }
}
