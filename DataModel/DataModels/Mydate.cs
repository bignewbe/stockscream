using CommonCSharpLibary.Serialize;
using System;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockScream.DataModels
{
    public class Mydate : CIXmlSerializable
    {
        [NotMapped]
        public DateTime Date { get { return new DateTime(Year, Month, Day); } }

        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public Mydate()
        {
            Year = 3000;
            Month = 1;
            Day = 1;
        }
        public Mydate(DateTime date)
        {
            Year = date.Year;
            Month = date.Month;
            Day = date.Day;
        }
        public Mydate(string dateStr, string format = null)
        {
            DateTime date = string.IsNullOrEmpty(format) ? DateTime.Parse(dateStr) : DateTime.ParseExact(dateStr, format, CultureInfo.InvariantCulture);
            Year = date.Year;
            Month = date.Month;
            Day = date.Day;
        }
    }
}
