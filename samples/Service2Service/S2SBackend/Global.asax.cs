using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.IdentityModel.Logging;

namespace S2SBackend
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        // constants related to this site: S2SBackend
        public const string Address = "http://localhost:39274/";
        public const string Authority = "https://testingsts.azurewebsites.net/";
        public const string Audience1 = "http://S2SBackend";
        public const string Audience2 = "http://S2SBackend/";
        public const string ClientId = "api-002";
        public const string Endpoint = Address + @"api/AccessTokenProtected/ProtectedApi";
        public const string LogDir = @"C:\Logs\SAL";
        public const string SiteName = "S2SBackend";
        public const string Thumbprint = "27B911151A9CAC9868D83FAF37CAFF6D74B54E2A";

        protected void Application_Start()
        {
            IdentityModelEventSource.ShowPII = true;
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
