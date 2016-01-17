using CommonCSharpLibary.CommonClass;
using CommonCSharpLibary.CommonFuntionality;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using StockScream.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace StockScream.Identity
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public Plan CurrentPlan { get { return PlanHistory.LastOrDefault(); } }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Mydate Birthday { get; set; }
        public MyCreditcard CreditCard { get; set; }
        public MyAddress BillingAddress { get; set; }
        public Token Token { get; set; }
        public virtual ICollection<Plan> PlanHistory { get; set; }

        [NotMapped]
        public SerializableStringDictionary FiltersFA { get; set; }
        [NotMapped]
        public SerializableStringDictionary FiltersW { get; set; }
        [NotMapped]
        public SerializableStringDictionary FiltersM { get; set; }

        public string XmlFilterFA
        {
            get
            {
                return CFacility.SerializeToXmlStr(FiltersFA);
            }
            set
            {
                FiltersFA = CFacility.DeserializeFromXmlStr<SerializableStringDictionary>(value) ??
                    new SerializableStringDictionary();
            }
        }
        public string XmlFiltersW
        {
            get
            {
                return CFacility.SerializeToXmlStr(FiltersW);
            }
            set
            {
                FiltersW = CFacility.DeserializeFromXmlStr<SerializableStringDictionary>(value) ??
                    new SerializableStringDictionary();
            }
        }
        public string XmlFiltersM
        {
            get
            {
                return CFacility.SerializeToXmlStr(FiltersM);
            }
            set
            {
                FiltersM = CFacility.DeserializeFromXmlStr<SerializableStringDictionary>(value) ??
                    new SerializableStringDictionary();
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
        public ApplicationUser()
        {
            FirstName = LastName = string.Empty;
            Birthday = new Mydate();
            CreditCard = new MyCreditcard();
            BillingAddress = new MyAddress();
            PlanHistory = new List<Plan>();
            FiltersFA = new SerializableStringDictionary();
            FiltersW =  new SerializableStringDictionary();
            FiltersM =  new SerializableStringDictionary();
            Token = new Token();
        }
    }
}
