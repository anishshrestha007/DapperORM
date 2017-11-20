using Bango;
using Bango.Models.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Bango.Rbac
{
    [TableDetail("rbac_session_log", "is_deleted", "login_datetime")]
    //[ComboFields("id", name: "name_np")]
    public class SessionLogModel : Models.ModelBase
    {
        public SessionLogModel()
        {
            this.DefaultLangFile = "SessionLog";
        }
        [Key]
        public int? id { get; set; }
        public int? client_id { get; set; } = null;
        [Required]
        public int user_id { get; set; }
        [Required]
        public DateTime login_datetime { get; set; }
        [Required]
        public DateTime expire_datetime { get; set; }
        [Required]
        [MaxLength(200)]
        public string token { get; set; }
        [MaxLength(200)]
        public string asp_net_session_id { get; set; }
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

        public override void initUniqueFields()
        {
            base.initUniqueFields();
            UniqueFields.Clear();
        }
    }
}