using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Security.Claims;

namespace StockScream.Hubs
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class AuthorizeClaimsAttribute : AuthorizeAttribute
    {
        protected override bool UserAuthorized(System.Security.Principal.IPrincipal user)
        {
            if (user == null) {
                throw new ArgumentNullException("user");
            }

            var principal = user as ClaimsPrincipal;
            var authenticated = principal?.Identity.IsAuthenticated ?? false;
            return authenticated;

            //if (principal != null)
            //{
            //    Claim authenticated = principal.FindFirst(ClaimTypes.Authentication);
            //    if (authenticated != null && authenticated.Value == "true")
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            //else
            //{
            //    return false;
            //}
        }

        public override bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
        {
            return base.AuthorizeHubConnection(hubDescriptor, request);
        }

        public override bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
        {
            //hubIncomingInvokerContext.Hub.Context.Request;
            return base.AuthorizeHubMethodInvocation(hubIncomingInvokerContext, appliesToMethod);
        }
    }
}
