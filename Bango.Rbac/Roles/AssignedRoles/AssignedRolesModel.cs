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

namespace Bango.Rbac.Roles.AssignedRoles
{
    [TableDetail("rbac_role_roles", "is_deleted", "assigned_role_id")]
    [ComboFields("id", "code", name:"name_np")]
    public class AssignedRolesModel :ModelBase
    {
        public AssignedRolesModel()
        {
            this.DefaultLangFile = "role";
        }
        [Key]
        public int? id { get; set; }
        public int? client_id { get; set; } = null;
        [Required]
        public int role_id { get; set; }
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
            UniqueFields.Add("assigned_role_id", new string[] { "client_id", "role_id", "assigned_role_id", "deleted_uq_code" }, GetLang("assigned_role") + " " + GetLang("Duplicate_Msg"));
        }

    }
}