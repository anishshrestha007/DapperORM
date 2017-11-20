using Bango.Models;
using Bango.Models.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Event
{
    [TableDetail("charities_events_details", "is_deleted", "")]
    [ComboFields("id", "code", "full_name")]
    public class EventModel : ModelBase
    {
        [Key]
        public int? id { get; set; }
        public int? client_id { get; set; }
        public string full_name { get; set; }
        public string organizer_name { get; set; }
        public string duration { get; set; }
        public DateTime? hosted_date{ get; set; }
        public string hosted_time{ get; set; }
        public string description{ get; set; }
        public int org_type { get; set; }
        public int event_type_id { get; set; }
        public string event_location { get; set;}
        public string contact_person_name { get; set; }
        public string contact_number { get; set; }

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