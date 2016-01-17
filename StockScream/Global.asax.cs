using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CommonCSharpLibary.StockScream;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using System.IO;
using System.Web.Http;

using StockScream.Services;
using StockScream.Identity;
using StockScream.Models;

namespace StockScream
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            MyDbInitializer.InitializeDb(new ApplicationDbContext());

            AreaRegistration.RegisterAllAreas();

            // Manually installed WebAPI 2.2 after making an MVC project.
            GlobalConfiguration.Configure(WebApiConfig.Register);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //StockScream.Models.ApplicationDbInitializer.ResetDatabases();
            var filename = Path.GetFileName(ConfigurationManager.AppSettings["stockMapPath"]);
            var stockMapPath = Server.MapPath(ConfigurationManager.AppSettings["stockMapPath"]);
            var logPath = Server.MapPath(ConfigurationManager.AppSettings["logPath"]);
            StockScream.Services.Globals.InitializeGlobals(stockMapPath, logPath);
        }
        
        //// Redirect http requests to the https URL
        //protected void Application_BeginRequest()
        //{
        //    if (!Context.Request.IsSecureConnection) {
        //        // This is an insecure connection, so redirect to the secure version
        //        UriBuilder uri = new UriBuilder(Context.Request.Url);
        //        uri.Scheme = "https";
        //        if (uri.Port > 32000 && uri.Host.Equals("localhost")) {
        //            // Development box - set uri.Port to 44300 by default
        //            uri.Port = 44300;
        //        }
        //        else {
        //            uri.Port = 443;
        //        }

        //        Response.Redirect(uri.ToString());
        //    }
        //}
        
        protected void Session_Start(object sender, EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated && Session["profile"] == null) {
                var userName = HttpContext.Current.User.Identity.Name;
                var db = MongoConfig.OpenUsers();
                var profile = db.FindAsync(u => u.Email == userName).Result.ToListAsync().Result;
                Session["profile"] = profile[0];
            }
        }
    }
}
