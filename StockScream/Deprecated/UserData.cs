using CommonCSharpLibary.CommonFuntionality;
using CommonCSharpLibary.Serialize;
using StockScream.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockScream.Services
{
    public class UserData : CIXmlSerializable
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Mydate Birthday { get; set; }
        public MyCreditcard CreditCard { get; set; }
        public MyAddress BillingAddress { get; set; }
        public Plan SubscribPlan { get; set; }
        public virtual List<Plan> PlanHistory { get; set; }

        //public ICollection<string> Roles { get; set; }
        [NotMapped]
        public Dictionary<string, string> FiltersFA { get; set; }
        [NotMapped]
        public Dictionary<string, string> FiltersW { get; set; }
        [NotMapped]
        public Dictionary<string, string> FiltersM { get; set; }

        public UserData()
        {
            SubscribPlan = null;
            Birthday = null;
            CreditCard = null;
            BillingAddress = null;

            PlanHistory = new List<Plan>();
            FiltersFA = new Dictionary<string, string>();
            FiltersM = new Dictionary<string, string>();
            FiltersM = new Dictionary<string, string>();
        }
    }    
}