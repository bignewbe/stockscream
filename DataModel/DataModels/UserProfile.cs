using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using CommonCSharpLibary.CustomExtensions;
using System.Globalization;
using System.Xml;
using System.Collections;
using CommonCSharpLibary.Serialize;
using MongoDB.Bson;
using System.ComponentModel;

namespace StockScream.DataModels
{
    //1. field added => no extra work
    //2. field removed => handled by ExtraElements
    //3. field renamed => handled by EndInt to copy from old value in ExtraElements
    //4. field type changed => need custum serializer if type is not compatible 
    [BsonIgnoreExtraElements]
    public class UserProfile : CIXmlSerializable, ISupportInitialize 
    {
        [BsonElement("_id")]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        //used when property/field is deleted. the old ones can still be deserialized and put here
        [BsonExtraElements]
        public IDictionary<string, object> ExtraElements { get; set; }  

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Mydate Birthday { get; set; }
        public MyCreditcard CreditCard { get; set; }
        public MyAddress BillingAddress { get; set; }
        public SubscriptionPlan SubscribPlan { get; set; }
        public ICollection<SubscriptionPlan> PlanHistory { get; set; }

        //public ICollection<string> Roles { get; set; }
        public Dictionary<string, string> FiltersFA { get; set; }
        public Dictionary<string, string> FiltersW { get; set; }
        public Dictionary<string, string> FiltersM { get; set; }
        
        public UserProfile()
        {
            Id = Email = FirstName = LastName = string.Empty;
            Birthday = new Mydate();
            CreditCard = new MyCreditcard();
            BillingAddress = new MyAddress();
            SubscribPlan = new SubscriptionPlan();
            PlanHistory = new List<SubscriptionPlan>();
            FiltersFA = new Dictionary<string, string>();
            FiltersW = new Dictionary<string, string>();
            FiltersM = new Dictionary<string, string>();
            //Roles = new HashSet<string>();
        }
        
        public void BeginInit()
        {
        }

        //executed after serialization. useful when a property is renamed => We can mannually create new property from old property in ExtraElments
        public void EndInit()
        {
            object nameValue;
            if (ExtraElements != null && ExtraElements.TryGetValue("Filters", out nameValue)) {
                var value = nameValue as IDictionary;
                var keys = value.Keys.Cast<string>().ToList();
                var values = value.Values.Cast<string>().ToList();
                for (int i = 0; i < keys.Count; i++) {
                    FiltersFA.Add(keys[i], values[i]);
                }
                ExtraElements.Remove("Filters");
            }

            ////following code demonstrate how we initialize birthday after we renamed it from "Birth" to "Birthday"
            //object nameValue;
            //if (ExtraElements.TryGetValue("Birth", out nameValue)) {
            //    var value = nameValue as IDictionary;
            //    Birthday = new Mydate { Year = (int)value["Year"], Month = (int)value["Month"], Day = (int)value["Day"] };
            //    ExtraElements.Remove("Birth");
            //}
        }
    }

    public class SubscriptionPlan : CIXmlSerializable
    {
        public Mydate StartDate { get; set; }
        public string ChargePeriod { get; set; }
        public string AccountType { get; set; }
        public SubscriptionPlan()
        {
            StartDate = new Mydate();
        }
    }
}
