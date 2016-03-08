using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using CommonCSharpLibary.CommonClass;
using CommonCSharpLibary.CommonFuntionality;
using Newtonsoft.Json;
using System;
using CommonCSharpLibary.Facility;

namespace StockScream.DataModels
{
    public class UserProfile
    {
        public int Id { get; set; }
        public Plan CurrentPlan { get { return PlanHistory.LastOrDefault(); } }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Mydate Birthday { get; set; }
        public MyCreditcard CreditCard { get; set; }
        public MyAddress BillingAddress { get; set; }
        public Token Token { get; set; }

        // define one-to-many relation using virtual and ICollection
        public virtual ICollection<Plan> PlanHistory { get; set; }

        [NotMapped]
        public Dictionary<string, List<string>> Portfolio { get; set; }
        public string JsonPortofolio
        {
            get
            {
                return JsonConvert.SerializeObject(Portfolio);
            }
            set
            {
                Type type = typeof(Dictionary<string, List<string>>);
                Portfolio = string.IsNullOrEmpty(value)? new Dictionary<string, List<string>>() :
                    JsonConvert.DeserializeObject(value, type) as Dictionary<string, List<string>>;
            }
        }

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
                //called when write to database
                return CFacility.SerializeToXmlStr(FiltersFA);   
            }
            set
            {
                //called when retrieve data from database
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

        static bool AddUpdateFilter(SerializableStringDictionary dic, string name, string command)
        {
            if (dic == null) return false;

            if (!dic.ContainsKey(name) || dic[name] != command)
            {
                dic.Add(name, command);
                return true;
            }

            return false;
        }

        static bool RemoveFilter(SerializableStringDictionary dic, string name)
        {
            if (dic == null) return false;
            if (dic.ContainsKey(name))
            {
                dic.Remove(name);
                return true;
            }
            return false;
        }

        public bool AddToPortfolio(string name, params string[] symbols)
        {
            if (symbols == null) return false;
            if (!Portfolio.ContainsKey(name)) {
                Portfolio.Add(name, symbols.ToList());
                return true;
            }

            var isAdded = false;
            foreach(var symbol in symbols) {
                if (!Portfolio[name].Contains(symbol)) {
                    Portfolio[name].Add(symbol);
                    isAdded = true;
                }
            }
            return isAdded;
        }

        public bool RemoveFromPortfolio(string name, params string[] symbols)
        {
            if (symbols == null || !Portfolio.ContainsKey(name)) return false;

            var isRemoved = false;
            foreach (var symbol in symbols) {
                if (Portfolio[name].Contains(symbol)) {
                    Portfolio[name].Remove(symbol);
                    isRemoved = true;
                }
            }
            return isRemoved;
        }
        
        public bool AddUpdateFilterFA(string name, string command)
        {
            return AddUpdateFilter(FiltersFA, name, command);
        }
        public bool AddUpdateFilterW(string name, string command)
        {
            return AddUpdateFilter(FiltersW, name, command);
        }
        public bool AddUpdateFilterM(string name, string command)
        {
            return AddUpdateFilter(FiltersM, name, command);
        }
        public bool RemoveFilterFA(string name)
        {
            return RemoveFilter(FiltersFA, name);
        }
        public bool RemoveFilterW(string name)
        {
            return RemoveFilter(FiltersW, name);
        }
        public bool RemoveFilterM(string name)
        {
            return RemoveFilter(FiltersM, name);
        }

        public UserProfile()
        {
            FirstName = LastName = string.Empty;
            Birthday = new Mydate();
            CreditCard = new MyCreditcard();
            BillingAddress = new MyAddress();
            Token = new Token();
            PlanHistory = new List<Plan>();
            FiltersFA = new SerializableStringDictionary();
            FiltersM = new SerializableStringDictionary();
            FiltersW = new SerializableStringDictionary();
        }
    }

    ////1. field added => no extra work
    ////2. field removed => handled by ExtraElements
    ////3. field renamed => handled by EndInt to copy from old value in ExtraElements
    ////4. field type changed => need custum serializer if type is not compatible 
    //[BsonIgnoreExtraElements]
    //public class UserProfile : CIXmlSerializable, ISupportInitialize 
    //{
    //    [BsonElement("_id")]
    //    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    //    public string Id { get; set; }

    //    //used when property/field is deleted. the old ones can still be deserialized and put here
    //    [BsonExtraElements]
    //    public IDictionary<string, object> ExtraElements { get; set; }  

    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string Email { get; set; }
    //    public Mydate Birthday { get; set; }
    //    public MyCreditcard CreditCard { get; set; }
    //    public MyAddress BillingAddress { get; set; }
    //    public SubscriptionPlan SubscribPlan { get; set; }
    //    public ICollection<SubscriptionPlan> PlanHistory { get; set; }

    //    //public ICollection<string> Roles { get; set; }
    //    public Dictionary<string, string> FiltersFA { get; set; }
    //    public Dictionary<string, string> FiltersW { get; set; }
    //    public Dictionary<string, string> FiltersM { get; set; }

    //    public UserProfile()
    //    {
    //        Id = Email = FirstName = LastName = string.Empty;
    //        Birthday = new Mydate();
    //        CreditCard = new MyCreditcard();
    //        BillingAddress = new MyAddress();
    //        SubscribPlan = new SubscriptionPlan();
    //        PlanHistory = new List<SubscriptionPlan>();
    //        FiltersFA = new Dictionary<string, string>();
    //        FiltersW = new Dictionary<string, string>();
    //        FiltersM = new Dictionary<string, string>();
    //        //Roles = new HashSet<string>();
    //    }

    //    public void BeginInit()
    //    {
    //    }

    //    //executed after serialization. useful when a property is renamed => We can mannually create new property from old property in ExtraElments
    //    public void EndInit()
    //    {
    //        object nameValue;
    //        if (ExtraElements != null && ExtraElements.TryGetValue("Filters", out nameValue)) {
    //            var value = nameValue as IDictionary;
    //            var keys = value.Keys.Cast<string>().ToList();
    //            var values = value.Values.Cast<string>().ToList();
    //            for (int i = 0; i < keys.Count; i++) {
    //                FiltersFA.Add(keys[i], values[i]);
    //            }
    //            ExtraElements.Remove("Filters");
    //        }

    //        ////following code demonstrate how we initialize birthday after we renamed it from "Birth" to "Birthday"
    //        //object nameValue;
    //        //if (ExtraElements.TryGetValue("Birth", out nameValue)) {
    //        //    var value = nameValue as IDictionary;
    //        //    Birthday = new Mydate { Year = (int)value["Year"], Month = (int)value["Month"], Day = (int)value["Day"] };
    //        //    ExtraElements.Remove("Birth");
    //        //}
    //    }
    //}

    //public class SubscriptionPlan : CIXmlSerializable
    //{
    //    public Mydate StartDate { get; set; }
    //    public string ChargePeriod { get; set; }
    //    public string AccountType { get; set; }
    //    public SubscriptionPlan()
    //    {
    //        StartDate = new Mydate();
    //    }
    //}
}
