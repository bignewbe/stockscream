using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.UI;
using System.Threading.Tasks;
using CommonCSharpLibary.TACommon;
using CommonCSharpLibary.CustomExtensions;
using Microsoft.AspNet.Identity.Owin;
using StockScream.Identity;
using StockScream.Services;
using PortableCSharpLib;

namespace StockScream.Controllers
{
    public class DataController : Controller
    {
        //private StockDbContext db = new StockDbContext();
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        
        [Route("data/download/{filename}")]
        public ActionResult Download(string filename)
        {
            var path = Server.MapPath("/App_Data/" + filename);
            if (!System.IO.File.Exists(path)) return null;

            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(filename));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]  //does not work with OuputCache
        [OutputCache(Duration = 900, Location = OutputCacheLocation.Client, VaryByParam = "symbol")]
        public async Task<ActionResult> RequestQuote(string symbols)
        {
            var searchResult = await Global.me.StockClient.RequestQuote(symbols.Split(';'));
            if (searchResult == null) return null;

            var lst = new List<object>();
            foreach (QuoteCommon q in searchResult) {
                var item = new
                {
                    Time = q._time.Select(t => t.GetJsonFromUnixTime()).ToList(),
                    High = q._high,
                    Low = q._low,
                    Open = q._open,
                    Close = q._close,
                    Volume = q._volume,
                    EMA = q._ema,
                };
                lst.Add(item);
            }
            return Json(lst[0], JsonRequestBehavior.AllowGet);
        }

        //this function should not be necessary since we have already passed Mapping information to client
        [OutputCache(Duration = int.MaxValue, Location = System.Web.UI.OutputCacheLocation.Client, VaryByParam = "table")]
        public JsonResult GetTableItem(string table)
        {
            var map = Global.me.MapStock;
            var tableTitles = new List<string>();
            if (table == "all" || table == "tables")
                tableTitles.AddRange(map.TableToItems.Keys.ToList());
            else if (map.TableToItems.ContainsKey(table))
                tableTitles.Add(table);

            var lst = new List<object>();
            if (table == "all") {
                foreach (var item in map.ItemSpecial) {
                    var data = new { Item = item, Category = "none" };
                    lst.Add(data);
                }
            }

            foreach (var tableTitle in tableTitles) {
                foreach (var item in map.TableToItems[tableTitle]) {
                    if (map.ItemFinancials.Contains(item) || map.ItemAnalyst.Contains(item)) {
                        var data = new { Item = item, Category = tableTitle };
                        lst.Add(data);
                    }
                }
            }
            var keys = new List<string> { "Item", "Category" };
            var items = new List<string> { "Item", "Category" };
            var result = new { Data = lst, Items = items, Keys = keys };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}