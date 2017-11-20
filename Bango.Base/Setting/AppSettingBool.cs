using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Base.Setting
{
    public class AppSettingBool : AppSetting<bool>
    {
        public AppSettingBool(string appSettingKey, bool defaultValue, bool autoLoad)
            : base(appSettingKey, defaultValue, autoLoad)
        {

        }
        public AppSettingBool(string appSettingKey, bool defaultValue)
            : base(appSettingKey, defaultValue)
        {
        }
        public AppSettingBool(string appSettingKey)
            : base(appSettingKey)
        {

        }
        public override bool LoadValue()
        {
            _value = false;
            string val = AppSettingValue;
            if (val != null)
            {
                _value = val.Trim().ToLower() != "true" ? false : true;
                return true;
            }
            return DefaultValue;
        }
    }
}
