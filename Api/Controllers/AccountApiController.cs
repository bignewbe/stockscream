using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using StockScream.Identity;
using System.Diagnostics;
using Microsoft.Owin.Testing;
using StockScream.BindingModels;
using StockScream.ViewModels;
using StockScream.ResultModels;
using StockScream.DataModels;
using System.Net.Mail;
using System.Net;
using System.Collections.Concurrent;
using StockScream.Services;

namespace StockScream.ApiControllers
{
    //public class Global
    //{
    //    public static Global Instance;
    //    static Global()
    //    {
    //        Instance = new Global();
    //    }

    //    public ConcurrentDictionary<string, Token> ConfirmationTokens { get; set; }
    //    public TcpStockClient StockClient { get; set; }

    //    public Global()
    //    {
    //        ConfirmationTokens = new ConcurrentDictionary<string, Token>();
    //    }
    //}

    [Authorize]
    [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
    [RoutePrefix("api/Account")]
    public class AccountApiController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;
        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        public AccountApiController()
        {
        }

        public AccountApiController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        /// <summary>
        /// test api server is running
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("TestAnonymous")]
        public IHttpActionResult TestAnonymous()
        {
            return Ok("Hello, Web Api works!");
        }

        /// <summary>
        /// test bearer token is effective
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("TestToken")]
        public IHttpActionResult TestToken()
        {
            return Ok("Hello, Web Api works!");
        }

        // GET api/AccountApi/UserInfo
        [HttpGet]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }
        
        // POST api/user/login

        // POST api/AccountApi/Logout => this is useless, token is still valid after SignOut. only solution is to delete token from client side
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/AccountApi/ManageInfo?returnUrl=%2F&generateState=true
        [HttpGet]
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
            };
        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
                return GetErrorResult(result);

            return Ok();
        }

        [HttpPost]
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
                return GetErrorResult(result);

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("EmailCode")]
        public async Task<IHttpActionResult> RequestEmailCode(string email)
        {
            try
            {
                if (!ModelState.IsValidField("Email"))
                    return BadRequest(ModelState);

                //1. generate 6 digit code and save to database
                var token = new Token(6, 60);

                //2. add to database
                if (!Global.Instance.ConfirmationTokens.ContainsKey(email))
                    Global.Instance.ConfirmationTokens.TryAdd(email, token);
                else
                    Global.Instance.ConfirmationTokens[email] = token;

                //3. send to client
                var message = new IdentityMessage { Body = token.AuthToken, Subject = "confirmation code", Destination = email };
                await UserManager.EmailService.SendAsync(message);
                return Ok();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return InternalServerError(e);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("RegisterEmail")]
        public async Task<IHttpActionResult> RegisterEmail(RegisterEmailBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //check email confirmation code
                if (!Global.Instance.ConfirmationTokens.ContainsKey(model.Email) || 
                    !Global.Instance.ConfirmationTokens[model.Email].CheckConfirmationCode(model.Code))
                    return BadRequest("Invalid confirmation code");

                //remove token from database
                Token token;
                Global.Instance.ConfirmationTokens.TryRemove(model.Email, out token);

                //create user in database
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                    return GetErrorResult(result);

                result = UserManager.AddToRole(user.Id, "Free");
                if (!result.Succeeded)
                    return GetErrorResult(result);

                //confirm Email to allow user to login website. note we already confirm the email using confirmation code
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                result = await UserManager.ConfirmEmailAsync(user.Id, code);
                if (!result.Succeeded)
                    return GetErrorResult(result);
                
                // Auto login after registrаtion (successful user registration should return access_token)
                var loginResult = await this.LoginEmail(new LoginViewModel { Email = model.Email, Password = model.Password });
                return loginResult;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
                return InternalServerError(e);
            }
        }
        
        [HttpPost]
        [AllowAnonymous]
        [Route("LoginEmail")]
        public async Task<IHttpActionResult> LoginEmail(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return this.BadRequest("Invalid user data");

            // Invoke the "token" OWIN service to perform the login (POST /api/token)
            //var testServer = TestServer.Create<Startup>();
            var requestParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", model.Email),
                new KeyValuePair<string, string>("password", model.Password)
            };
            var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
            //var tokenServiceResponse = await testServer.HttpClient.PostAsync("/Token", requestParamsFormUrlEncoded);

            var handler = new HttpClientHandler { CookieContainer = new CookieContainer() };
            var httpClient = new HttpClient(handler);
            var tokenServiceResponse = await httpClient.PostAsync("/Token", requestParamsFormUrlEncoded);

            return this.ResponseMessage(tokenServiceResponse);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("PhoneCode")]
        public IHttpActionResult RequestPhoneCode(string phone)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //1. generate 6 digit code
                // var code = 123456;
                //2. send to phone
                
                return Ok();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return InternalServerError(e);
            }
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("RegisterPhone")]
        public async Task<IHttpActionResult> RegisterPhone(RegisterPhoneBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = new ApplicationUser { UserName = model.Phone, PhoneNumber = model.Phone };

                IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                    return GetErrorResult(result);

                UserManager.AddToRole(user.Id, "Free");
                                
                return Ok();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return InternalServerError(e);
            }
        }

        ////
        //// POST: /Account/Login
        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IHttpActionResult> LoginEmail(LoginViewModel model, string returnUrl)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // Require the user to have a confirmed email before they can log on.
        //    // var user = await UserManager.FindByNameAsync(model.Email);
        //    var user = UserManager.Find(model.Email, model.Password);
        //    if (user == null) return View();

        //    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
        //    {
        //        string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account-Resend");

        //        // Uncomment to debug locally  
        //        ViewBag.Link = callbackUrl;
        //        ViewBag.errorMessage = "You must have a confirmed email to log on. "
        //                             + "The confirmation token has been resent to your email account.";
        //        return View("Error");
        //    }

        //    // This doesn't count login failures towards account lockout
        //    // To enable password failures to trigger account lockout, change to shouldLockout: true
        //    var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
        //    var isVerified = await SignInManager.HasBeenVerifiedAsync();

        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            await this.LoadUserProfileByEmail(model.Email);
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid login attempt.");
        //            return View(model);
        //    }
        //}

        [HttpGet]
        [AllowAnonymous]
        [Route("ConfirEmail")]
        public async Task<IHttpActionResult> ConfirmEmail(ConfirmEmailBindingModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await UserManager.ConfirmEmailAsync(model.UserId, model.Code);

            if (result.Succeeded)
                return Ok();

            return Content(System.Net.HttpStatusCode.Unauthorized, "Failed to confirm email");
        }       
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UserManager.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }

}
