using Bango.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Bango.Base.List;
using System.Net.Http;
using System.Web.Http.Cors;

namespace Bango.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PrivateController : ApiController
    {
        protected virtual DynamicDictionary GetJosnRequestAsDictionary()
        {
            string result = Request.Content.ReadAsStringAsync().Result;
            return Conversion.ToDictionaryFromJson(result);
        }
        protected virtual DynamicDictionary GetFormRequestAsDictionary()
        {
            string result = Request.Content.ReadAsStringAsync().Result;
            return Conversion.ToDictionaryFormKeyValue(result);
        }


        protected virtual String GetRequestAsString()
        {
            string result = Request.Content.ReadAsStringAsync().Result;
            return result;
        }
        protected virtual DynamicDictionary GetQueryAsDictionary(){
            DynamicDictionary dic = new DynamicDictionary();
            foreach (KeyValuePair<string, string> item in Request.GetQueryNameValuePairs())
            {
                dic.Add(item.Key, (object)item.Value);
            }
            return dic;
        }
        
    }
}
