using CommonCSharpLibary.CommonClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockScream.DataModels
{
    public class Plan : CIXmlSerializable
    {
        static public List<string> AccountTypes = new List<string> { "Admin", "Free", "Basic", "Advanced", "Ultimate" };
        
        public int Id { get; set; }
        public string StartDate { get; set; }
        public string ChargePeriod { get; set; }
        public string AccountType { get; set; }

        [NotMapped]
        public bool IsValid { get { return AccountTypes.Contains(AccountType); } }

        [NotMapped]
        public Mydate Date { get { return new Mydate(StartDate); } }

        public Plan()
        {
            AccountType = ChargePeriod = StartDate = string.Empty;
        }
        public Plan(DateTime startDate, string period, string type)
        {
            StartDate = startDate.ToShortDateString();
            ChargePeriod = period;
            AccountType = type;
        }
    }
}
