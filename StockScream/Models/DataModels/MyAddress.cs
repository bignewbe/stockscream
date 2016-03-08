using CommonCSharpLibary.CommonClass;

namespace StockScream.DataModels
{
    public class MyAddress : CIXmlSerializable
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string Postcode { get; set; }
        public MyAddress()
        {
            Country = State = City = Street = HouseNumber = Postcode = string.Empty;
        }
    }
}
