using Bango;
using Bango.Models.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Bango.Rbac.User
{
    [TableDetail("rbac_user", "is_deleted", "name_np")]
    [ComboFields("id", name: "name_np")]
    public class UserModel : Bango.Rbac.UserModel
    {
        public UserModel()
        {
            this.DefaultLangFile = "user";

        }
        //[Key]
        //public int? id { get; set; }
        //public int? client_id { get; set; } = null;
        //[Required]
        //[MaxLength(20)]
        //public string username { get; set; }
        //[MaxLength(200)]
        //public string name_np { get; set; }
        //[MaxLength(200)]
        //public string name_en { get; set; }
        [MaxLength(200)]
        public string email { get; set; }
        [MaxLength(500)]
        public string photo_path { get; set; }
        public int? staff_id { get; set; }
        //[MaxLength(200)]
        //public string password { get; set; }
        //public DateTime? last_logged_in_datetime { get; set; }
        //[MaxLength(50)]
        //public string last_logged_in_ip { get; set; }
        //public bool? status { get; set; } = true;
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
            base.DefaultLangFile = "user";
            base.initUniqueFields();
            UniqueFields.Clear();
            UniqueFields.Add("username", new string[] { "client_id", "username", "deleted_uq_code" }, GetLang("username") + " " + GetLang("Duplicate_Msg"));
        }
    }
}