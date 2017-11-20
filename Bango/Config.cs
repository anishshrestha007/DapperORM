using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bango.Base.Setting;

namespace Bango
{
    //loads confi from web.config
    public class Config
    {
        public AppSettingBool IsProduction = new AppSettingBool("MyroConfig:IsProduction");
        public AppSetting<string> ProductionDbConnection = new AppSetting<string>("MyroConfig:ProductionDbConnection");
        public AppSetting<string> DevDbConnection = new AppSetting<string>("MyroConfig:DevDbConnection");
        public string DbConnectionString
        {
            get{
                if (IsProduction.Value)
                    return ProductionDbConnection.Value;
                else
                    return DevDbConnection.Value;
            }
            
        }
    }
}
