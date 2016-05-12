using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.IO;
using System.Web.Http;
using System.Linq;
using System.Collections.Generic;

namespace StockScream
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // Manually installed WebAPI 2.2 after making an MVC project.
            GlobalConfiguration.Configure(WebApiConfig.Register);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            InitializeGlobal();
        }

        private void InitializeGlobal()
        {
            //StockScream.Models.ApplicationDbInitializer.ResetDatabases();
            //var stockMapPath = Server.MapPath(ConfigurationManager.AppSettings["stockMapPath"]);
            //var logPath = Server.MapPath(ConfigurationManager.AppSettings["logPath"]);
            //StockScream.Services.Global.Initialize(stockMapPath, logPath);

            StockScream.Services.Global.Instance.InitializeNLLogger(Server.MapPath(ConfigurationManager.AppSettings["logPath"]));
            StockScream.Services.Global.Instance.ConnectQuoteServer(ConfigurationManager.AppSettings["ip_quoteServer"], int.Parse(ConfigurationManager.AppSettings["port_quoteServer"]));
            StockScream.Services.Global.Instance.ConnectStockServer(ConfigurationManager.AppSettings["ip_stockServer"], int.Parse(ConfigurationManager.AppSettings["port_stockServer"]));
            StockScream.Services.Global.Instance.LoadStockMeta(Server.MapPath(ConfigurationManager.AppSettings["stockMapPath"]));

            var db = new StockDbContext();
            var symbols1 = (from s in db.StockFinancials select s.Symbol).ToList();
            var symbols2 = (from s in db.PPStocks select s.Symbol).ToList();
            StockScream.Services.Global.Instance.AvailableSymbolsForTechAnalysis = new HashSet<string>(symbols1.Intersect(symbols2));
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
            //if (HttpContext.Current.User.Identity.IsAuthenticated && Session["profile"] == null) {
            //    var userName = HttpContext.Current.User.Identity.Name;
            //    var db = MongoConfig.OpenUsers();
            //    var profile = db.FindAsync(u => u.Email == userName).Result.ToListAsync().Result;
            //    Session["profile"] = profile[0];
            //}
        }
    }
}
