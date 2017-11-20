using Bango.Base.List;
using Bango.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Rbac
{
    public class ResponseAuth :ResponseBase, IResponseAuth
    {
        public ResponseAuth()
            : this(false, string.Empty)
        {

        }
        public ResponseAuth(bool success, string token)
            : this(success, token, "", string.Empty)
        {
        }

        public ResponseAuth(bool success, string token, string message)
            : this(success, token, message, string.Empty)
        {
        }
        public ResponseAuth(bool success, string token, string message, string errorCode)
            : base(success, message, errorCode)
        {
            this.token = token;
        }
        public DynamicDictionary PushValidationErrors(DynamicDictionary errorList)
        {
            return null;
        }
        public string token { get; set; }
        //[JsonIgnore]
        public int? user_id { get; set; }
        public string email { get; set; }
    }
}
