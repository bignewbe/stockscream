using System.Web;
using System.Web.Optimization;

namespace StockScream
{
   public class BundleConfig
   {
      // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
      public static void RegisterBundles(BundleCollection bundles)
      {
         bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                     "~/Scripts/jquery.unobtrusive-ajax.js",
                     "~/Scripts/jquery-{version}.js",
                     "~/Scripts/jquery.signalR-{version}.js"));

         bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                     "~/Scripts/jquery.validate*"));

         // Use the development version of Modernizr to develop with and learn from. Then, when you're
         // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
         bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                     "~/Scripts/modernizr-*"));

         bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                   "~/Scripts/bootstrap-table.js",
                   //"~/Scripts/bootstrap-table-all.js",
                   //"~/Scripts/bootstrap-table-locale-all.js",
                   "~/Scripts/jquery-linedtextarea.js",
                   "~/Scripts/facility.js",
                   "~/Scripts/bootstrap.js",
                   "~/Scripts/respond.js"));

         bundles.Add(new StyleBundle("~/Content/css").Include(
                   "~/Content/bootstrap-table.css",
                   "~/Content/bootstrap.css",
                   "~/Content/jquery-linedtextarea.css",
                   "~/Content/site.css"));

         // Set EnableOptimizations to false for debugging. For more information,
         // visit http://go.microsoft.com/fwlink/?LinkId=301862
         BundleTable.EnableOptimizations = false;
      }
   }
}
