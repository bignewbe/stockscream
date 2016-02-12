using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using StockScream.Services;
using StockScream.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using StockScream.BindingModels;

namespace StockScream.ApiControllers
{
    [Authorize]
    [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
    [RoutePrefix("api/Filter")]
    public class DataApiController : ApiController
    {
        ApplicationDbContext _applicationDbcontext;
        ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                //return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                _applicationDbcontext = _applicationDbcontext ?? new ApplicationDbContext();
                return _userManager ?? new ApplicationUserManager(new UserStore<ApplicationUser>(_applicationDbcontext));
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpPost]
        [Route("SaveFilterFA")]
        public async Task<IHttpActionResult> SaveFilterFA(KeyValueBindingModel kv)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
                return InternalServerError();
            
            if (user.Profile.AddUpdateFilterFA(kv.Key, kv.Value))
                MyDbInitializer.SaveDbContext(_applicationDbcontext);

            return Ok(user.Profile.FiltersFA.Count);
        }

        [HttpPost]
        [Route("AddToPortfolio")]
        public async Task<IHttpActionResult> AddToPortfolio(KeyValuesBindingModel kv)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
                return InternalServerError();

            if (user.Profile.AddToPortfolio(kv.Key, kv.Values.ToArray()))
                MyDbInitializer.SaveDbContext(_applicationDbcontext);

            return Ok(user.Profile.Portfolio.Count);
        }

        [HttpPost]
        [Route("RemoveFromPortfolio")]
        public async Task<IHttpActionResult> RemoveFromPortfolio(KeyValuesBindingModel kv)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
                return InternalServerError();

            if (user.Profile.RemoveFromPortfolio(kv.Key, kv.Values.ToArray()))
                MyDbInitializer.SaveDbContext(_applicationDbcontext);

            return Ok(user.Profile.Portfolio.Count);
        }

    }
}
