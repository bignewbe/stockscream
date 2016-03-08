using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StockScream.Startup))]
namespace StockScream
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();

            //app.Map("/signalr", map =>
            //{
            //    map.UseCors(CorsOptions.AllowAll);

            //    map.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions()
            //    {
            //        Provider = new QueryStringOAuthBearerProvider()
            //    });

            //    var hubConfiguration = new HubConfiguration
            //    {
            //        Resolver = GlobalHost.DependencyResolver,
            //    };
            //    map.RunSignalR(hubConfiguration);
            //});
        }
    }
}
