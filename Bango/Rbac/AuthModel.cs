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
    //[TableDetail("rbac_user", "is_deleted", "name_np")]
    //[ComboFields("id", name:"name_np")]
    public class AuthModel :  Models.ModelBase
    {
        public AuthModel()
        {
            this.DefaultLangFile = "AuthModel";

        }
        public int? client_id { get; set; } = null;
        [Required]
        [MaxLength(20)]
        public string username { get; set; }
        [MaxLength(200)]
        public string password { get; set; }
        public string message { get; set; }
        public bool? status { get; set; } = false;

    }
}