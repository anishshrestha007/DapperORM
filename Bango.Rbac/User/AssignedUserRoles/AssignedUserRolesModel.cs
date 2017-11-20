using Bango.Models;
using Bango.Models.Attributes;
using Newtonsoft.Json;
using System;

namespace Bango.Rbac.User.AssignedUserRoles
{
    [TableDetail("rbac_user_roles", "is_deleted", "assigned_role_id")]
    //[ComboFields("id", "code", "name_np")]
    public class AssignedUserRolesModel :ModelBase
    {
        public AssignedUserRolesModel()
        {
            this.DefaultLangFile = "user";
        }
        [Key]
        public int? id { get; set; }
        public int? client_id { get; set; } = null;
        [Required]
        public int user_id { get; set; }
        [Required]
        public int assigned_role_id { get; set; }
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
            UniqueFields.Add("assigned_role_id", new string[] { "client_id", "user_id", "assigned_role_id", "deleted_uq_code" }, GetLang("assigned_role") + " " + GetLang("Duplicate_Msg"));
        }

    }
}