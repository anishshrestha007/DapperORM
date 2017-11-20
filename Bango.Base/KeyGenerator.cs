using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Base
{
    public class KeyGenerator
    {
        private static Random r = new Random();
        public string UniqueKey { get; set; }
        public string DateTimeFormat { get; set; }
        
        public delegate string GetInitialDelegate();
        public KeyGenerator()
        {
            DateTimeFormat = "yyyMMdd_hhmmss_fffff";
        }
        public string GenerateUniqueKey( GetInitialDelegate getInitial)
        {
            return GenerateUniqueKey(getInitial());
        }
        public virtual string GenerateUniqueKey(string initial)
        {
            return initial.Trim().ToLower() + "_" + DateTime.Now.ToString(DateTimeFormat) + "_" + GetRandom().ToString();
        }
        public string GenerateUniqueKey()
        {
            return GenerateUniqueKey(GetInitial());
        }

        public virtual string GetInitial()
        {
            return string.Empty;
        }

        public int GetRandom()
        {
            return r.Next(1000, 9999);
        }
    }
}
