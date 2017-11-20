using Bango;
using Bango.Models;
using Bango.Models.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Bango.Rbac.Roles
{
    [TableDetail("rbac_role_master", "is_deleted", "code")]
    [ComboFields("id", "code", name: "name_np")]
    public class RolesModel : ModelBase
    {
        public RolesModel()
        {
            this.DefaultLangFile = "role";
        }

        [Key]
        public int? id { get; set; }
        public int? client_id { get; set; }
        [Required]
        [MaxLength(100)]
        public string code { get; set; }
        [Required]
        [MaxLength(200)]
        [ActualFieldName("name")]
        public string name_np { get; set; }
        [Required]
        [MaxLength(200)]
        [ActualFieldName("name")]
        public string name_en { get; set; }
        [MaxLength(500)]
        public string description { get; set; }
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
        public bool? is_system_defined { get; set; } = false;
        [JsonIgnore]
        public int? deleted_uq_code { get; set; } = 1;

        public override void initUniqueFields()
        {
            UniqueFields.Clear();
            base.DefaultLangFile = "role";
            UniqueFields.Add("code", new string[] { "client_id", "code", "deleted_uq_code" }, GetLang("code") + " " + GetLang("Duplicate_Msg"));

        }



    }
}