using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using StockScream.Models;
using CommonCSharpLibary.StockScream;
using CommonCSharpLibary.CommonFuntionality;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MongoDB.Driver;
using System.Web.UI;
using System.Linq;
using StockScream.Identity;
using StockScream.Services;
using Microsoft.AspNet.Identity.EntityFramework;
using CommonCSharpLibary.CommonClass;
using System.Collections.Specialized;
using StockScream.ViewModels;
using StockScream.DataModels;

namespace StockScream.Controllers
{
    public class SearchController : Controller
    {
        private StockDbContext db = new StockDbContext();

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


        // fundamental analysis
        public async Task<ActionResult> Index()
        {
            var dic = new SerializableStringDictionary();
            if (User.Identity.IsAuthenticated)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                dic = user.Profile.FiltersFA;
            }

            var model = new FAModel { Map = Global.me.MapStock, SavedFilters = dic.ToDictionary() };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(Duration = 10000, Location = OutputCacheLocation.Client, VaryByParam = "command")]
        public ActionResult RunCommand(string command)
        {
            var map = Global.me.MapStock;
            var items = map.ExtractItemsFromCommand(command) as List<string>;
            if (items == null) return null;
            items.Insert(0, "Stock");          //We always want to show stock           

            var tup = map.GenerateSQLCommandHeader(items.ToArray());
            if (tup == null) return null;
            var header = tup.Item1;            //sql command header
            var keys = tup.Item2;              //key that can be directily queried from SQL
            var mappedKeys = tup.Item3;        //mapped key returned from SQL query

            var filter = QueryFilter.ParseCommandStr(command);
            if (filter == null) return null;
            var sqlCommand = tup.Item1 + filter.GetQueryString(map);

            Debug.WriteLine(sqlCommand);

            //dynamic create a type to hold query results. type must be matched, otherwise we can not convert the returned results
            TypeBuilder builder = CFacility.CreateTypeBuilder("MyDynamicAssembly", "MyModule", "MyType");
            foreach (var key in mappedKeys)
            {
                if (map.MappedKeyTypes.ContainsKey(key))
                {
                    if (map.MappedKeyTypes[key] == "int")
                        CFacility.CreateAutoImplementedProperty(builder, key, typeof(Nullable<int>));
                    else
                        CFacility.CreateAutoImplementedProperty(builder, key, typeof(string));
                }
                else CFacility.CreateAutoImplementedProperty(builder, key, typeof(Nullable<float>));
            }
            Type resultType = builder.CreateType();

            //create the query task
            var taskQuery = db.Database.SqlQuery(resultType, sqlCommand).ToListAsync();

            var result = new { Data = taskQuery.Result, Items = items, Keys = mappedKeys };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveFilter(string name, string command)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null) return View("Error");

            if (user.Profile.AddUpdateFilterFA(name, command))
                MyDbInitializer.SaveDbContext(_applicationDbcontext);

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveFilter(string name)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null) return View("Error");

            if (user.Profile.RemoveFilterFA(name))
                MyDbInitializer.SaveDbContext(_applicationDbcontext);

            return RedirectToAction("Index");
        }
    }
}