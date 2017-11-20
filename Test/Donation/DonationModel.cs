using Bango.Models;
using Bango.Models.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Donation
{
    [TableDetail("donation_details", "is_deleted", "")]
    [ComboFields("id", "code", "")]
    public class DonationModel : ModelBase
    {
        [Key]
        public int? id { get; set; }
        public int? client_id { get; set; }

        public string approximate_value { get; set; }
        public string image { get; set; }
        public DateTime? pickup_datetime{ get; set; }
        public DateTime expiration_datetime { get; set; }
        public string description{ get; set; }
        public int posted_by { get; set; }
        public DateTime posted_date { get; set; }
        public string address { get; set; }
        [JsonIgnore]
        public int? created_by { get; set; }
        [JsonIgnore]
        public DateTime? created_on { get; set; }
        [JsonIgnore]
        public int? updated_by { get; set; }
        [JsonIgnore]
        public DateTime? updated_on { get; set; }
        [JsonIgnore]
        public int? deleted_by { get; set; }
        [JsonIgnore]
        public DateTime? deleted_on { get; set; }
        [JsonIgnore]
        public bool? is_deleted { get; set; } = false;
        [JsonIgnore]
        public int? deleted_uq_code { get; set; } = 1;

       public int? donation_type_id { get; set; }
        public int? donnar_type_id { get; set; }
 
        




    }
}