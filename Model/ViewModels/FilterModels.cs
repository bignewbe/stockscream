using System;

namespace StockScream.ViewModels
{
    public partial class StockFinancialModel
    {
        public Nullable<int> IndustryID { get; set; }
        public Nullable<int> SectorID { get; set; }
        public string Exch { get; set; }
        public Nullable<float> PETTM { get; set; }
        public Nullable<float> PEH5Y { get; set; }
        public Nullable<float> PEL5Y { get; set; }
        public Nullable<float> Beta { get; set; }
        public Nullable<float> P2STTM { get; set; }
        public Nullable<float> P2SMRQ { get; set; }
        public Nullable<float> P2TBMRQ { get; set; }
        public Nullable<float> P2CFTTM { get; set; }
        public Nullable<float> PInst { get; set; }
        public Nullable<float> DY { get; set; }
        public Nullable<float> DY5Y { get; set; }
    }
}