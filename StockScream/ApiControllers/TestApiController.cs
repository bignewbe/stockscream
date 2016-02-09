using CommonCSharpLibary.TACommon;
using CommonCSharpLibary.CustomExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using StockScream.Services;
using PortableCSharpLib;

namespace StockScream.ApiControllers
{
    public class TestAPIController : ApiController
    {
        //[HttpGet]
        //public IHttpActionResult Download(string filename)
        //{
        //    var path = Server.MapPath("/App_Data/" + filename);
        //    if (!System.IO.File.Exists(path)) return null;

        //    byte[] fileBytes = System.IO.File.ReadAllBytes(path);
        //    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(filename));
        //}

        /// <summary>
        /// get stock quotes from quote server
        /// </summary>
        /// <param name="symbols">';'separated stock symbols. for example: GOOG;msft will return quotes for Google and Microsoft </param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> RequestQuote(string symbols)
        {            
            var searchResult = await Global.me.StockClient.RequestQuote(symbols.Split(';'));
            if (searchResult == null) return null;

            var lst = new List<object>();
            foreach (QuoteCommon q in searchResult){
                var item = new
                {
                    Symbol = q.Symbol,
                    Time = q._time.Select(t => t.GetJsonFromUnixTime()).ToList(),
                    High = q._high,
                    Low = q._low,
                    Open = q._open,
                    Close = q._close,
                    Volume = q._volume,
                    EMA = q._ema
                };
                lst.Add(item);
            }
            return Json(lst);
        }

        //this function should not be necessary since we have already passed Mapping information to client
        [HttpGet]
        public IHttpActionResult GetTableItem(string table)
        {
            var map = Global.me.MapStock;
            var tableTitles = new List<string>();
            if (table == "all" || table == "tables")
                tableTitles.AddRange(map.TableToItems.Keys.ToList());
            else if (map.TableToItems.ContainsKey(table))
                tableTitles.Add(table);

            var lst = new List<object>();
            if (table == "all")
            {
                foreach (var item in map.ItemSpecial)
                {
                    var data = new { Item = item, Category = "none" };
                    lst.Add(data);
                }
            }

            foreach (var tableTitle in tableTitles)
            {
                foreach (var item in map.TableToItems[tableTitle])
                {
                    if (map.ItemFinancials.Contains(item) || map.ItemAnalyst.Contains(item))
                    {
                        var data = new { Item = item, Category = tableTitle };
                        lst.Add(data);
                    }
                }
            }
            var keys = new List<string> { "Item", "Category" };
            var items = new List<string> { "Item", "Category" };
            var result = new { Data = lst, Items = items, Keys = keys };
            return Json(result);
        }

        [HttpGet]
        [Authorize]
        public List<string> GetSymbols()
        {
            return new List<string> { "ABC", "GOOG" };
        }
    }
}
