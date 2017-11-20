using System;
namespace Bango.Helpers
{
    public interface IRegisterType
    {
        LightInject.ServiceContainer Container { get; }
        void Register();
    }
}
