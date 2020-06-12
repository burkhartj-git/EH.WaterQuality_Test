using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace EHWaterQuality.Models
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start()
        {
            try
            {
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(Server.MapPath("configs\\log4net.config")));
            }
            catch (Exception e)
            {
                throw new NullReferenceException("Exception Occured. log4net configuration could not be completed.", e);
            }

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error()
        {
            try
            {
                var applicationEnvironment = ConfigurationManager.AppSettings["Application.Environment"];
                if (applicationEnvironment != null && applicationEnvironment.Length > 0 && (applicationEnvironment.Contains("LOCAL") || applicationEnvironment.Contains("DEV")))
                    return;

                Exception e = Server.GetLastError();
                Response.Clear();
                HttpException httpException = e as HttpException;
                if (httpException != null)
                {
                    string action = null;
                    switch (httpException.GetHttpCode())
                    {
                        case 404:
                            action = "NotFound";
                            break;
                        case 500:
                            action = "InternalServerError";
                            break;
                    }
                    Server.ClearError();
                    var _lock = new Object();
                    string newurl;
                    lock (_lock)
                    {
                        UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
                        newurl = url.Action(action, "Error");
                    }

                    Response.Redirect(newurl);
                }
                else
                {
                    //other runtime errors !
                    Server.ClearError();

                    var _lock = new Object();
                    string newurl;
                    lock (_lock)
                    {
                        UrlHelper url = new UrlHelper(HttpContext.Current.Request.RequestContext);
                        newurl = url.Action("Generic", "Error");
                    }
                    Response.Redirect(newurl);
                }
            }
            catch
            {
                //ingore everything.
                //throw;
            }
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        protected void Session_Start()
        {
            if (ConfigurationManager.AppSettings["Application.LeftNav.StartCollapsed"] != null && ConfigurationManager.AppSettings["Application.LeftNav.StartCollapsed"] == "true")
            {
                EHWaterQuality.Utilities.SessionHelper.SideBarClosed = "1";
            }
        }

        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiConfig.UrlPrefixRelative);
        }
    }
}
