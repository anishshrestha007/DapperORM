using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Rbac
{
    public interface IUserModel : Models.IModel
    {
        int? id { get; set; }
        int? client_id { get; set; }
        string username { get; set; }
        string name_np { get; set; }
        string name_en { get; set; }
        string password { get; set; }
        DateTime? last_logged_in_datetime { get; set; }
        string last_logged_in_ip { get; set; }
        string description { get; set; }
        bool? status { get; set; }
    }
}
