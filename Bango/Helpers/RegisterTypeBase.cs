using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Helpers
{
    public class RegisterTypeBase : Bango.Helpers.IRegisterType
    {
        public ServiceContainer Container { get; private set; }
        public RegisterTypeBase(ServiceContainer container)
        {
            Container = container;
        }
        public RegisterTypeBase()
            : this(new ServiceContainer())
        {
        }
        public virtual void Register()
        {
            //Container.Register();
        }
    }
}
