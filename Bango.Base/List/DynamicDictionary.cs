using System;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Bango.Base.List
{
    public class DynamicDictionary : DynamicObject, ICloneable, IDictionary<string, object>
    {
        // The inner dictionary.
        protected Dictionary<string, object> dictionary
            = new Dictionary<string, object>();

        // This property returns the number of elements
        // in the inner dictionary.
        public int GetCount()
        {
            
            return dictionary.Count;
        }

        //public Dictionary<string, object>.KeyCollection GetProperties()
        //{
        //    return dictionary.Keys;
        //}
        //public Dictionary<TKey, TValue>.Enumerator AsEnumerable<TSource>()
        //{
        //    return dictionary.GetEnumerator();
        //}
        // If you try to get a value of a property 
        // not defined in the class, this method is called.
        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            string name = binder.Name.ToLower();

            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            return dictionary.TryGetValue(name, out result);
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            dictionary[binder.Name.ToLower()] = value;

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }
        public void Add(string name, object value)
        {
            SetValue(name, value);
        }

        public void SetValue(string name, object value)
        {
            dictionary[name.ToLower()] = value;
        }
        public T GetValue<T>(string name)
        {
            object val = GetValue(name);
            if(val != null)
            {
                return TypeHelper.ChangeType<T>(val);
            }
            return default(T);
        }
        public object GetValue(string name)
        {
            if (dictionary.ContainsKey(name.ToLower()))
            {
                return dictionary[name.ToLower()];
            }
            return null;
        }

        public int? GetValueAsInt(string name)
        {
            object obj = GetValue(name);
            if (obj == null)
                return null;
            else
                return Conversion.ToInt32(obj);
        }

        public string GetValueAsString(string name)
        {
            object obj = GetValue(name);
            if (obj == null)
                return null;
            else
                return obj.ToString();
        }

        public bool GetValueAsBoolean(string name)
        {
            object obj = GetValue(name);
            if (obj == null)
                return false;
            else
            {
                if(obj.ToString().Trim().ToLower() == "true")
                {
                    return true;
                }
                else if (obj.ToString().Trim().ToLower() == "1")
                {
                    return true;
                }
                else if (obj.ToString().Trim().ToLower() == "on")
                {
                    return true;
                }
            }
            return false;
        }
        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool ContainsKeyStartWith(string prefix)
        {
            foreach (string key in KeyList)
            {
                if (key.ToLower().Trim().StartsWith(prefix))
                    return true;
            }
            return false;
        }

        public bool ContainsKeySEndsWith(string prefix)
        {
            foreach (string key in KeyList)
            {
                if (key.ToLower().Trim().EndsWith(prefix))
                    return true;
            }
            return false;
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public ICollection KeyList
        {
            get { return dictionary.Keys; }
        }

        ICollection<string> IDictionary<string, object>.Keys
        {
            get
            {
                return dictionary.Keys;
            }
        }

        public ICollection<object> Values
        {
            get
            {
                return dictionary.Values;
            }
        }

        public int Count
        {
            get
            {
                return GetCount();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public object this[string key]
        {
            get
            {
                if(dictionary.ContainsKey(key))
                    return dictionary[key];
                return null;
            }
            set
            {
                dictionary[key] = value;
            }
        }

        public object Clone()
        {
            
            DynamicDictionary dic = new DynamicDictionary();
            foreach (KeyValuePair<string, object> item in dictionary)
            {
                dic.Add(item.Key, item.Value);
            }
            return dic;
        }

        public void Append(DynamicDictionary dic)
        {
            if(dic != null)
            {
                foreach(KeyValuePair<string, object> item in dic.dictionary)
                {
                    Add(item.Key, item.Value);
                }
            }
        }

        public bool Remove(string key)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary.Remove(key);
            }
            return true;
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            if(dictionary.ContainsKey(item.Key)){
                dictionary.Remove(item.Key);
            }
            return true;
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public bool TryGetValue(string key, out object value)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            string name = key.ToLower();

            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            return dictionary.TryGetValue(name, out value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            dictionary.Add(item.Key, item.Value);
        }
    }
}
