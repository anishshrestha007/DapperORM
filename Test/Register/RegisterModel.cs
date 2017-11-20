using Bango.Models;
using Bango.Models.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Models.Register
{
    [TableDetail("rbac_user", "is_deleted")]
    public class RegisterModel : ModelBase
    {
        [Key]
        public int? id { get; set; }
        public int? client_id { get; set; } = null;
        [MaxLength(20)]
        public string username { get; set; }
        [MaxLength(100)]
        public string name_en { get; set; }
        [MaxLength(100)]
        public string name_np { get; set; }
        [MaxLength(100)]
        public string password { get; set; }
        public DateTime last_logged_in_datetime { get; set; }
        public string last_logged_in_ip { get; set; }
        public string description { get; set; }
        public bool status { get; set; }
        public bool is_system_defined { get; set; }
        public int staff_if { get; set; }
        public string email { get; set; }
        public string photo_path { get; set; }
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
        public int? display_order { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string contact_number { get; set; }
    }
}