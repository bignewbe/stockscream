using CommonCSharpLibary.StockAnalysis;
using System.Collections.Generic;

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