using System;
using Bango.Models.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Bango.Rbac.Roles.AssignedRights
{
    [TableDetail("rbac_role_rights", "is_deleted", "assigned_right_id")]
    [ComboFields("id", "code", name: "name_np")]
    public class assignedrightsModel :Models.ModelBase
    {
        public assignedrightsModel()
        {
            this.DefaultLangFile = "role";
        }
        [Models.Attributes.Key]
        public int? id { get; set; }
        public int? client_id { get; set; } = null;
        [Models.Attributes.Required]
        public int role_id { get; set; }
        [Models.Attributes.Required]
        public int assigned_right_id { get; set; }
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
            base.initUniqueFields();
            UniqueFields.Clear();
            UniqueFields.Add("assigned_right_id", new string[] { "client_id", "role_id", "assigned_right_id", "deleted_uq_code" }, GetLang("assigned_role") + " " + GetLang("Duplicate_Msg"));
        }

    }
}