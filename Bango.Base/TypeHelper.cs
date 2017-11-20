using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Base
{
    public class TypeHelper
    {
        public static T ChangeType<T>(object value)
        {
            var t = typeof(T);

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return default(T);
                }
                else if (value.ToString().Trim().Length == 0)
                {
                    if (t.IsValueType == true) //only for numerice value return null othe wise data convert...
                        return default(T);
                }

                t = Nullable.GetUnderlyingType(t); ;
            }

            return (T)Convert.ChangeType(value, t);
        }

        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            //TODO:Shivashwor 14 March 2016 due while value =nuul rais error..
            if (value == null)
                return null;

            if (value.GetType().FullName == "Newtonsoft.Json.Linq.JArray")
            {
                string items = value.ToString();
                value = items;
            }

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                t = Nullable.GetUnderlyingType(t); 
            }
            //TODO:Shivashwor 26 nov 2015 ned for if value empty retrun nulll
            if (value == null)
                return null;
            else if (value.ToString().Trim().Length == 0 )
                if (t.IsValueType== true) //only for numerice value return null othe wise data convert...
                    return null;

            return Convert.ChangeType(value, t);
        }
    }
}
