using System;
namespace Bango.Base.Setting
{
    public interface ISettingBase<T>
        //where T : struct
    {
        bool LoadValue();
        T Value { get; set; }
        T DefaultValue { get; set; }
        bool AutoLoad { get; set; }
    }
}
