using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Bango.Base
{
    /// <summary>
    /// Class for holding all the general data related functions.
    /// </summary>
    public static class DataGeneral
    {
        #region "object disposing"
        public static void DisposeObject(DataTable dt)
        {
            if ((dt != null))
            {
                if (dt.Rows != null)
                    dt.Rows.Clear();
                dt.Clear();
                dt.Dispose();
                dt = null;
            }
        }

        public static void DisposeObject(IDataReader dataRdr)
        {
            if ((dataRdr != null))
            {
                dataRdr.Close();
                dataRdr.Dispose();
                dataRdr = null;
            }
        }

        public static void DisposeObject(IDbCommand cmd)
        {
            if ((cmd != null))
            {
                cmd.CommandText = string.Empty;
                cmd.Dispose();
                cmd = null;
            }
        }

        public static void DisposeObject(ref byte[] arr)
        {
            if ((arr != null))
            {
                System.Array.Clear(arr, 0, arr.Length);
                arr = null;
            }
        }

        public static void DisposeObject(ref ArrayList arrList)
        {
            if ((arrList != null))
            {
                arrList.Clear();
                arrList = null;
            }
        }

        public static void DisposeObject(ref object[] objArr)
        {
            if ((objArr != null))
            {
                //objArr.Clear(objArr, 0, objArr.Length);
                objArr = null;
            }
        }
        #endregion
        public static Dictionary<string, object> MergeDictionary(Dictionary<string, object> source, Dictionary<string, object> destination)
        {
            return MergeDictionary<string, object>(source, destination);
        }
        public static Dictionary<TKey, TValue> MergeDictionary<TKey, TValue>(Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> destination)
        {
            if (destination == null)
                destination = new Dictionary<TKey, TValue>();
            if (source == null)
                return destination;
            foreach (KeyValuePair<TKey, TValue> item in source)
            {
                if (destination.ContainsKey(item.Key))
                {
                    destination[item.Key] = item.Value;
                }
                else
                {
                    destination.Add(item.Key, item.Value);
                }
            }
            return destination;
        }
    }
}
