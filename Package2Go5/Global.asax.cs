using Package2Go5.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Package2Go5
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutoMapperWebConfiguration.Configure();
        }

        protected void Application_Error(object sender, EventArgs e) {
            Exception ex = Server.GetLastError();
            if (ex != null && ex.Message.Contains("Maximum request length exceeded"))
            {
                Server.ClearError();
                Response.Redirect("/UserProfile/Edit?e=1");
            }
        }
    }
}