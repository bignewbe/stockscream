using StockScream.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;

namespace StockScream.Filters
{
    /// <summary>
    /// executed before start of every action
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class LoadUserProfileAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            //if (HttpContext.Current.User.Identity.IsAuthenticated && filterContext.HttpContext.Session["profile"] == null)
            //{
            //    var userName = HttpContext.Current.User.Identity.Name;
            //    var db = MongoConfig.OpenUsers();
            //    var profile = db.FindAsync(u => u.Email == userName).Result.ToListAsync().Result;
            //    //var profile = await (await db.FindAsync(u => u.Email == userName)).ToListAsync();
            //    //if (profile != null && profile.Count == 1)
            //    filterContext.HttpContext.Session["profile"] = profile[0];
            //}
        }
    }
}
