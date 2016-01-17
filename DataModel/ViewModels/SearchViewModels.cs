using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonCSharpLibary.Stock;
using CommonCSharpLibary.StockScream;

namespace StockScream.ViewModels
{
    public class FAModel
    {
        public StockMapping Map { get; set; }
        public IDictionary<string, string> SavedFilters { get; set; }
    }

    public class TAModel
    {
        public WParamMapping Map { get; set; }
        public IDictionary<string, string> SavedFilters { get; set; }
    }
}