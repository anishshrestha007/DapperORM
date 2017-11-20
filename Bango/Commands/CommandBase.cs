using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Commands
{
    public class CommandBase<T> : ICommand<T>
    {
        public string Name
        {
            get;
            set;
        }

        public List<string> Errors
        {
            get;
            set;
        }

        public T Result
        {
            get;
            set;
        }

        public bool Execute()
        {
            if (!BeforeExecute())
                return false;
            if (!Executing())
                return false;
            if (!AfterExecute())
                return false;
            return true;
        }

        protected virtual bool BeforeExecute()
        {
            return true;
        }

        protected virtual bool Executing()
        {
            throw new NotImplementedException();
        }

        protected virtual bool AfterExecute()
        {
            return true;
        }
    }
}
