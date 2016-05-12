using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using StockScream.Identity;
using Microsoft.Owin.Security.OAuth;
using StockScream.Providers;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security;

namespace StockScream
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Code removed for brevity.

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);


            // Configure the application for OAuth based flow
            if (OAuthOptions == null)           //create OAuthOptions only once since we use TestServer.Create<Startup>()
            {
                PublicClientId = "self";
                OAuthOptions = new OAuthAuthorizationServerOptions
                {
                    TokenEndpointPath = new PathString("/Token"),
                    Provider = new ApplicationOAuthProvider(PublicClientId),
                    AuthorizeEndpointPath = new PathString("/api/AccountApi/ExternalLogin"),
                    AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(120),
                    AllowInsecureHttp = true,

                    //RefreshTokenProvider = new AuthenticationTokenProvider
                    //{
                    //    OnCreate = CreateRefreshToken,
                    //    OnReceive = ReceiveRefreshToken,
                    //},
                };
            }

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);            


            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            // This is the RickAndMSFT@gmail.com account which uses https://localhost:44306/

            //app.UseGoogleAuthentication(
            //    clientId:  ConfigurationManager.AppSettings["GoogClientID"],
            //    clientSecret:  ConfigurationManager.AppSettings["GoogClientSecret"]);
        }
        
        private static void CreateRefreshToken(AuthenticationTokenCreateContext context)
        {
            //var tokenExpiry = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ApiTokenExpiry"]);
            //var refreshTokenExpiry = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ApiRefreshTokenExpiry"]);

            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = DateTime.UtcNow.AddHours(24), // add 3 minutes to the access token expiry
            };

            var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

            OAuthOptions.RefreshTokenFormat.Protect(refreshTokenTicket);

            context.SetToken(context.SerializeTicket());
        }
        private static void ReceiveRefreshToken(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
        }
    }
}