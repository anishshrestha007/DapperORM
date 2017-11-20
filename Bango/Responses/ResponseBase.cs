using Bango.Base.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Responses
{
    public class ResponseBase : IResponseBase
    {
        public ResponseBase()
            : this(false, string.Empty)
        {

        }
        public ResponseBase(bool success, string message)
            : this(success, message, string.Empty)
        {
        }

        public ResponseBase(bool success, string message, string errorCode)
        {
            this.success = success;
            this.message = message;
            this.error_code = errorCode;
        }

        public bool success { get; set; }
        public string message { get; set; }
        public DynamicDictionary errors { get; set; } = new DynamicDictionary();
        public DynamicDictionary PushErrors(List<string> errorList)
        {
            int cnt = 1;
            foreach(string s in errorList)
            {
                errors.Add(cnt.ToString(), s);
            }
            return errors;
        }
        public DynamicDictionary PushErrors(DynamicDictionary errorList)
        {
            foreach (string s in errorList.KeyList)
            {
                errors.Add(s, errorList.GetValue(s));
            }
            return errors;
        }
        //public object data { get; set; }
        public string error_code { get; set; }

    }
}
