using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Base.List
{
    /// <summary>
    /// Extension of Dictionary<TKey, TValue>, if the value doesnot exists then it adds a new value.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DictionaryFx<TKey, TValue> : Dictionary<TKey, TValue>
    {
        #region Member variables.
        #endregion

        #region Constructors & Finalizers.
        public DictionaryFx()
        {
        }
        #endregion

        #region Nested Enums, Structs, and Classes.
        #endregion

        #region Properties
        public virtual new TValue this[TKey key]
        {
            get
            {

                //if (this.ContainsKey(key))
                //{
                return base[key];
                //}

                //return 
            }
            set
            {
                if (this.ContainsKey(key))
                    base[key] = value;
                else
                    this.Add(key, value);
            }
        }

        #endregion

        #region Methods
        #endregion
    }
}
