using Bango.Models;
using Bango.Models.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bango.Rbac.appManagement.Products
{
    [TableDetail("app_products", "is_deleted", "code")]
    [ComboFields("id", "code", "name_np")]
    public class ProductModel :ModelBase
    {
        public ProductModel()
        {
          this.DefaultLangFile = "productlist";
        }
        [Key]
        public Int16? id { get; set; }
       
        [Required]
        [MaxLength(10)]
        public string code { get; set; }
        [Required]
        [MaxLength(100)]
        [ActualFieldName("name")]
        public string name_en { get; set; }
        [Required]
        [MaxLength(100)]
        [ActualFieldName("name")]
        public string name_np { get; set; }
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
        [JsonIgnore]
        public int? deleted_uq_code { get; set; } = 1;
        
        public override void initUniqueFields()
        {
            base.initUniqueFields();
            base.UniqueFields.Clear();
            UniqueFields.Add("code", new string[] { "code", "deleted_uq_code" }, GetLang("code") + " " + GetLang("Duplicate_Msg"));
        }
    }
}