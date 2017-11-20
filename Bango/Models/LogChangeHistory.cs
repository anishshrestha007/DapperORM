using Bango.Attributes;
using Bango.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Models
{
    [TableDetail("log_change_history")]
    public class LogChangeHistory : ModelBase
    {

        [Key]
        public int? id { get; set; }
        public int? client_id { get; set; }
        public string table_name { get; set; }
        //public string old_values { get; set; }
        public string changed_values { get; set; }
        public int? user_id { get; set; }
        public string os_computer_name { get; set; }
        public string os_user_name { get; set; }
        public string os_ipaddress { get; set; }
        public string os_useragent { get; set; }
        public string activity_type { get; set; }
        [CurrentTimestamp]
        public DateTime? activity_datetime { get; set; }
    }
}
