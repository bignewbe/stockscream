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

namespace StockScream.ApiControllers
{
    public class FilterApiController : ApiController
    {
        //IUserDataServices _userProfileServices;
        //MyUserManager _unitOfWork;
        //private ApplicationUserManager _userManager;
        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        var context = Request.Properties["MS_HttpContext"] as System.Web.HttpContext;
        //        context.
        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}

        //public UserProfileController()
        //{
        //    var dbName = "Users";
        //    var connectionStr = ConfigurationManager.ConnectionStrings["MongoDbConnection"].ConnectionString;
        //    _userProfileServices = new UserDataServices(connectionStr, dbName);
        //}

        [HttpPost]
        public IHttpActionResult SaveFilterFA(string name, string command)
        {
            ApplicationUser usr = null;
            using (var usrManager = new MyUserManager<ApplicationUser, ApplicationDbContext>())
            {
                usr = usrManager.EntityRepository.GetByID(User.Identity.GetUserId());
            }

            //we have to find the user in database
            if (usr == null)
                return BadRequest("Unable to save filter for un-registered user");

            if (usr.FiltersFA.ContainsKey(name) && usr.FiltersFA[name] == command)
                return Ok();

            if (!usr.FiltersFA.ContainsKey(name))
                usr.FiltersFA.Add(name, command);
            else
                usr.FiltersFA[name] = command;

            using (var usrManager = new MyUserManager<ApplicationUser, ApplicationDbContext>())
            {
                //copy from user and perform update
                usrManager.EntityRepository.Update(usr);
            }

            return Ok();
        }
    }
}
