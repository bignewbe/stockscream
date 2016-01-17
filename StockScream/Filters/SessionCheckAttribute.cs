using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace StockScream.Filters
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class SessionCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            //string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            var session = filterContext.HttpContext.Session;
            if (session.IsNewSession)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    var userName = HttpContext.Current.User.Identity.Name;
                }
                ////Redirect
                //var url = new UrlHelper(filterContext.RequestContext);
                //var loginUrl = url.Content("~/Error/SessionTimeout");
                //filterContext.HttpContext.Response.Redirect(loginUrl, true);
            }
        }
    }
}
