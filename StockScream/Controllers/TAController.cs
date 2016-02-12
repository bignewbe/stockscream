using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using CommonCSharpLibary.CommonFuntionality;
using CommonCSharpLibary.Stock;
using StockScream.Models;
using CommonCSharpLibary.CustomExtensions;
using System.Diagnostics;
using Microsoft.AspNet.Identity;
using StockScream.Identity;
using StockScream.Services;
using StockScream.ViewModels;
using CommonCSharpLibary.CommonClass;
using Microsoft.AspNet.Identity.EntityFramework;

namespace StockScream.Controllers
{
    public class TAController : Controller
    {
        StockDbContext _stockDbContext = new StockDbContext();
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

        public async Task<ActionResult> Index()
        {
            var dic = new SerializableStringDictionary();
            if (User.Identity.IsAuthenticated)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                dic = user.Profile.FiltersW;
            }
            var model = new TAModel { Map = Global.me.MapWParam, SavedFilters = dic.ToDictionary() };

            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]  //does not work with OuputCache
        //[OutputCache(Duration=900, Location = OutputCacheLocation.Client, VaryByParam = "command", VaryByParam = "date")]
        public async Task<ActionResult> RunCommand(string date, string command)
        {
            var map = Global.me.MapStock;
            EnumParseError error = EnumParseError.OK;
            int lineNumber = -1;

            ///////////////////////////////////////////////////////////////////////////
            //create header
            var items = map.ExtractItemsFromCommand(command) as List<string>;
            if (items == null) {
                error = EnumParseError.ItemExtractionFailed;
                return ConstructReturnResult(error, lineNumber);
            }
            items.Insert(0, "Stock");          //We always want to show stock           

            var tup = map.GenerateSQLCommandHeader(items.ToArray());
            if (tup == null) {
                error = EnumParseError.ItemFailedToCreateHeader;
                return ConstructReturnResult(error, lineNumber);
            }
            var header = tup.Item1;                  //sql command header
            var keys = tup.Item2;                    //key that can be directily queried from SQL
            var mappedKeys = tup.Item3.ToList();     //mapped key returned from SQL query

            //header += "\nAND s.Symbol in ('AAPL', 'GOOG')";
            //Debug.WriteLine(header);

            ////////////////////////////////////////////////////////////////////////////
            //parse command
            var wp = WParam.ParseCommandStr(command, out error, out lineNumber);
            if (wp == null) {
                return ConstructReturnResult(error, lineNumber);
            }
            wp.QueryDate = date;

            ///////////////////////////////////////////////////////////////////////////
            //search W
            var searchResult = await Global.me.StockClient.SearchStocks(wp);

            if (searchResult == null) {
                error = EnumParseError.FailedToMatchAnyResults;
                return ConstructReturnResult(error, lineNumber);
            }
            else if (searchResult.GetType() == typeof(string)) {            //error message returned
                return Json(new { Msg = searchResult, Data = "", Items = "", Keys = "" }, JsonRequestBehavior.AllowGet);
            }

            //////////////////////////////////////////////////////////////////////////
            //query database
            //header += "\nAND s.Symbol in ('AAPL', 'GOOG')";
            var b = new StringBuilder();
            b.Append("AND s.Symbol in (");
            foreach (var symbol in (List<string>)searchResult) {
                b.Append(string.Format("'{0}',", symbol));
            }
            b.Remove(b.Length - 1, 1);
            b.Append(")");

            var sqlCommand = header + b.ToString();
            Debug.WriteLine(sqlCommand);

            //dynamic create a type to hold query results. type must be matched, otherwise we can not convert the returned results
            TypeBuilder builder = CFacility.CreateTypeBuilder("MyDynamicAssembly", "MyModule", "MyType");

            for (int i = 0; i < mappedKeys.Count; i++) {
                var key = mappedKeys[i];
                //if (key.Contains("_i"))
                //    key = key.Replace("_i", string.Empty);
                //if (key.Contains("_s"))
                //    key = key.Replace("_s", string.Empty);

                if (map.MappedKeyTypes.ContainsKey(key)) {
                    if (map.MappedKeyTypes[key] == "int")
                        CFacility.CreateAutoImplementedProperty(builder, mappedKeys[i], typeof(Nullable<int>));
                    else
                        CFacility.CreateAutoImplementedProperty(builder, mappedKeys[i], typeof(string));
                }
                else CFacility.CreateAutoImplementedProperty(builder, mappedKeys[i], typeof(Nullable<float>));
                //mappedKeys[i] = key;
            }
            Type resultType = builder.CreateType();

            //create the query task
            var taskQuery = _stockDbContext.Database.SqlQuery(resultType, sqlCommand).ToListAsync();
            return Json(new { Msg = "OK", Data = taskQuery.Result, Items = items, Keys = mappedKeys }, JsonRequestBehavior.AllowGet);
        }
        
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveFilterW(string name, string command)
        {
            //var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            //var manager = new ApplicationUserManager(store);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null) return View("Error");

            if (user.Profile.AddUpdateFilterW(name, command))
                MyDbInitializer.SaveDbContext(_applicationDbcontext);

            return RedirectToAction("SearchW");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveFilterW(string name)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null) return View("Error");

            if (user.Profile.RemoveFilterW(name))
                MyDbInitializer.SaveDbContext(_applicationDbcontext);

            return RedirectToAction("SearchW");
        }

        [OutputCache(Duration = 900, Location = OutputCacheLocation.Client, VaryByParam = "symbol")]
        public async Task<ActionResult> StockDetails(string symbol)
        {
            if (symbol == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            var quotes = await Global.me.StockClient.RequestQuote(new List<string> { symbol });
            if (quotes == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            ViewBag.Symbol = symbol;
            return View(quotes[0]);
        }

        #region facility
        string GetErrorMsg(EnumParseError errorCode, int lineNumber)
        {
            switch (errorCode) {
                case EnumParseError.ItemExtractionFailed:
                    return "Faied to understand SELECT section. Are you missing double quotes?";
                case EnumParseError.ItemFailedToCreateHeader:
                    return "Faied to understand SELECT section. Is there any non supported item? Please select items via 'Add Select Parameters'";
                case EnumParseError.ItemMissingColon:
                    return string.Format("line {0}: Are you missing ':' after item?", lineNumber);
                case EnumParseError.ItemNotRecognized:
                    return string.Format("line {0}: not supported item found.", lineNumber);
                case EnumParseError.ComparatorLeftIsNotK:
                case EnumParseError.ComparatorNotValid:
                    return string.Format("line {0}: filter experession must be 'k=some value, k<some value, or k>some value' ", lineNumber);
                case EnumParseError.FailedToMatchAnyResults:
                    return string.Format("Filter does not find any result");
                case EnumParseError.OK:
                default:
                    return "OK";
            }
        }
        ActionResult ConstructReturnResult(EnumParseError errorCode, int lineNumber)
        {
            var errorMsg = GetErrorMsg(errorCode, lineNumber + 1);                   //convert line number starting from 1 iso 0
            var result = new { Msg = errorMsg, Data = "", Items = "", Keys = "" };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}






//public class MyUserManager
//{
//    DbContext _applicationDbcontext;
//    public DbContext Dbcontext
//    {
//        get
//        {
//            if (_applicationDbcontext == null)
//                _applicationDbcontext = new ApplicationDbContext(); 
//            return _applicationDbcontext;
//        }
//        private set
//        {
//            _applicationDbcontext = value;
//        }
//    }
//    ApplicationUserManager _userManager;
//    public ApplicationUserManager UserManager
//    {
//        get
//        {
//            //return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
//            if (_userManager == null)
//                _userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(Dbcontext));

//            return _userManager ?? new ApplicationUserManager(new UserStore<ApplicationUser>(Dbcontext));
//        }
//        private set
//        {
//            _userManager = value;
//        }
//    }

//    public void SaveContext()
//    {
//        MyDbInitializer.SaveDbContext(Dbcontext);
//    }
//}
