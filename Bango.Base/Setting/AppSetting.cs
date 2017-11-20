using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Bango.Base.Setting
{
    public class AppSetting<T> : SettingBase<T>
    {
        protected string _appSettingKey;

        public virtual string AppSettingKey
        {
            get
            {
                return _appSettingKey;
            }
        }
        public override T Value
        {
            get
            {
                return _value;
            }
        }

        public AppSetting(string appSettingKey, T defaultValue, bool autoLoad)
        {
            DefaultValue = defaultValue;
            _appSettingKey = appSettingKey;
            AutoLoad = autoLoad;
            if (autoLoad)
            {
                LoadValue();
            }
        }

        public AppSetting(string appSettingKey, T defaultValue)
            : this(appSettingKey, defaultValue, true)
        {
        }
        public AppSetting(string appSettingKey)
            : this(appSettingKey, default(T), true)
        {

        }
        public override bool LoadValue()
        {
            string val = AppSettingValue;
            if (val != null)
            {

                _value = (T)Convert.ChangeType(val, typeof(T));
                return true;
            }
            return false;
            
        }
        
        protected string AppSettingValue
        {
            get
            {
                if (WebConfigurationManager.AppSettings.AllKeys.Contains<string>(AppSettingKey))
                {
                    return WebConfigurationManager.AppSettings[AppSettingKey];
                }
                return null;
            }
        }
    }
}
