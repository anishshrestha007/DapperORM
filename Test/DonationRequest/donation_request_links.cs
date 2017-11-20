using Bango.Models;
using Bango.Models.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.DonationRequest
{
    [TableDetail("donation_request_links", "is_deleted")]
    [ComboFields("id", "donation_id", "donation_request_id")]
    public class donation_request_links : ModelBase
    {
        [Key]
        public int? id { get; set; }
        public int? client_id { get; set; }
        [Required]
        [MaxLength(10)]
        public int? donation_id { get; set; }
        public int? requested_user_id { get; set; }
        public string description { get; set; }
       // public int? display_order { get; set; }
        public bool? status { get; set; } = true;
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
    }
}