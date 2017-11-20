using Bango.Models;
using Bango.Models.Attributes;
using Newtonsoft.Json;
using System;
namespace Bango.Rbac.User.AssignedUserRights
{
    [TableDetail("rbac_user_rights", "is_deleted", "assigned_right_id")]
    [ComboFields("id", "code", "name_np")]
    public class AssignedUserRightsModel :ModelBase
    {
        public AssignedUserRightsModel()
        {

        }

        [Key]
        public int? id { get; set; }
        public int? client_id { get; set; } = null;
        [Required]
        public int user_id { get; set; }
        [Required]
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
            base.UniqueFields.Clear();
            UniqueFields.Add("assigned_right_id", new string[] { "client_id", "user_id", "assigned_right_id", "deleted_uq_code" }, GetLang("assigned_right") + " " + GetLang("Duplicate_Msg"));
        }

    }
}