using CommonCSharpLibary.CommonClass;

namespace StockScream.DataModels
{
    public class MyCreditcard : CIXmlSerializable
    {
        public bool IsValid { get; set; }
        public string NameOnCard { get; set; }
        public string Type { get; set; }
        public string Number { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
        public MyCreditcard()
        {
            IsValid = false;
            NameOnCard = Type = Number = string.Empty;
            ExpMonth = 1;
            ExpYear = 1970;
        }
    }
}
