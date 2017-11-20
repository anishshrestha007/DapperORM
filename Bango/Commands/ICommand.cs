using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Commands
{
    public interface ICommand<T>
    {
        string Name { get; set; }
        List<string> Errors { get; set; }
        T Result { get; set; }
        /// <summary>
        /// Executes the task to be done by the Helpers & return the result 
        /// </summary>
        /// <typeparam name="bool">Sucess or Fail</typeparam>
        /// <returns></returns>
        bool Execute();
    }
}
