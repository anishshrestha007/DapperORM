using Dapper;
using Bango.Base.List;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Base
{
    public static class Conversion
    {

        private static void CheckIfColumnExists(DataRow dRow, string columnName)
        {
            if (dRow.Table.Columns.IndexOf(columnName) == -1)
                throw new Exception("Column Name doesnot exists.");
        }

        /// <summary>
        /// Converts the data in the Row's column to 32 bit Integer.
        /// </summary>
        /// <param name="dRow">Data Row</param>
        /// <param name="columnName">Column Name from which data will be acquired.</param>
        /// <returns>Returns 32 bit integer.</returns>
        public static int ToInt32(DataRow dRow, string columnName)
        {
            CheckIfColumnExists(dRow, columnName);

            int retVal = 0;

            try
            {
                int.TryParse(dRow[columnName].ToString(), out retVal);
            }
            catch
            {
                retVal = 0;
            }
            return retVal;
        }
        public static string ToString(object value)
        {
            if (value == null)
                return string.Empty;
            return value.ToString();
        }
        /// <summary>
        /// Converts the data in the Row's column to string.
        /// </summary>
        /// <param name="dRow">Data Row</param>
        /// <param name="columnName">Column Name from which data will be acquired.</param>
        /// <returns>Returns string.</returns>
        public static string ToString(DataRow dRow, string columnName)
        {
            CheckIfColumnExists(dRow, columnName);

            string retVal = string.Empty;

            try
            {
                retVal = dRow[columnName].ToString();
            }
            catch
            {
                retVal = string.Empty;
            }

            return retVal;
        }

        /// <summary>
        /// Converts the data in the Row's column to double.
        /// </summary>
        /// <param name="dRow">Data Row</param>
        /// <param name="columnName">Column Name from which data will be acquired.</param>
        /// <returns>Returns double.</returns>
        public static double ToDouble(DataRow dRow, string columnName)
        {
            CheckIfColumnExists(dRow, columnName);

            double retVal = 0;

            try
            {
                double.TryParse(dRow[columnName].ToString(), out retVal);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        /// <summary>
        /// Converts the data in the Row's column to float.
        /// </summary>
        /// <param name="dRow">Data Row</param>
        /// <param name="columnName">Column Name from which data will be acquired.</param>
        /// <returns>Returns float.</returns>
        public static float ToFloat(DataRow dRow, string columnName)
        {
            CheckIfColumnExists(dRow, columnName);

            float retVal = 0;

            try
            {
                float.TryParse(dRow[columnName].ToString(), out retVal);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        /// <summary>
        /// Converts the data in the Row's column to double.
        /// </summary>
        /// <param name="dRow">Data Row</param>
        /// <param name="columnName">Column Name from which data will be acquired.</param>
        /// <returns>Returns double.</returns>
        public static DateTime ToDateTime(DataRow dRow, string columnName)
        {
            CheckIfColumnExists(dRow, columnName);

            DateTime retVal = new DateTime();

            try
            {
                DateTime.TryParse(dRow[columnName].ToString(), out retVal);
            }
            catch
            {
                retVal = new DateTime();
            }

            return retVal;
        }
        public static DateTime ToDateTime(object date)
        {
            return ToDateTime(Conversion.ToString(date));
        }
        public static DateTime ToDateTime(string date)
        {

            DateTime retVal = new DateTime();

            try
            {
                DateTime.TryParse(date, out retVal);
            }
            catch
            {
                retVal = new DateTime();
            }

            return retVal;
        }

        /// <summary>
        /// Converts the data in the Row's column to double.
        /// </summary>
        /// <param name="dRow">Data Row</param>
        /// <param name="columnName">Column Name from which data will be acquired.</param>
        /// <returns>Returns double.</returns>
        public static DateTime ToDateTime(DataRow dRow, string columnName, string format)
        {
            CheckIfColumnExists(dRow, columnName);

            DateTime retVal = new DateTime();

            try
            {
                DateTime.TryParseExact(dRow[columnName].ToString(), format
                    , System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat, System.Globalization.DateTimeStyles.None, out retVal);
            }
            catch
            {
                retVal = new DateTime();
            }

            return retVal;
        }

        /// <summary>
        /// Converts the data in the Row's column to decimal.
        /// </summary>
        /// <param name="dRow">Data Row</param>
        /// <param name="columnName">Column Name from which data will be acquired.</param>
        /// <returns>Returns decimal.</returns>
        public static decimal ToDecimal(DataRow dRow, string columnName)
        {
            CheckIfColumnExists(dRow, columnName);

            decimal retVal = 0;

            try
            {
                decimal.TryParse(dRow[columnName].ToString(), out retVal);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }


        /// <summary>
        /// Converts the string value to decimal.
        /// </summary>        
        /// <param name="value">string value to be converted to decimal.</param>
        /// <returns>Returns decimal.</returns>
        public static decimal ToDecimal(string value)
        {
            decimal retVal = 0;

            try
            {
                decimal.TryParse(value, out retVal);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        public static decimal ToDecimal(object value)
        {
            return ToDecimal(Conversion.ToString(value));
        }

        /// <summary>
        /// Converts the string value to double.
        /// </summary>        
        /// <param name="value">string value to be converted to double.</param>
        /// <returns>Returns double.</returns>
        public static double ToDouble(string value)
        {
            double retVal = 0;

            try
            {
                double.TryParse(value, out retVal);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        /// <summary>
        /// Converts the string value to float.
        /// </summary>        
        /// <param name="value">string value to be converted to float.</param>
        /// <returns>Returns float.</returns>
        public static float ToFloat(string value)
        {
            float retVal = 0;

            try
            {
                float.TryParse(value, out retVal);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        /// <summary>
        /// Converts the string value to integer.
        /// </summary>        
        /// <param name="value">string value to be converted to integer.</param>
        /// <returns>Returns integer.</returns>
        public static int ToInt32(object value)
        {
            int retVal = 0;
            if (value != null)
            {
                retVal = ToInt32(value.ToString());
            }
            return retVal;
        }

        /// <summary>
        /// Converts the string value to integer.
        /// </summary>        
        /// <param name="value">string value to be converted to integer.</param>
        /// <returns>Returns integer.</returns>
        public static int ToInt32(string value)
        {
            int retVal = 0;

            try
            {
                int.TryParse(value, out retVal);
            }
            catch
            {
                retVal = 0;
            }

            return retVal;
        }

        /// <summary>
        /// Converts the data in the Row's column to boolean value.
        /// </summary>
        /// <param name="dRow">Data Row</param>
        /// <param name="columnName">Column Name from which data will be acquired.</param>
        /// <returns>bool a boolean value.</returns>
        public static Boolean ToBoolean(DataRow dRow, string columnName)
        {
            CheckIfColumnExists(dRow, columnName);

            bool retVal = false;

            try
            {
                retVal = ToBoolean(dRow[columnName].ToString());
            }
            catch
            {
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Converts the string data to boolean. if string value = ture or equal to 1 then return true else false.
        /// </summary>
        /// <param name="value">The value which has to be converted to boolean.</param>
        /// <returns>bool a boolean value.</returns>
        public static Boolean ToBoolean(string value)
        {

            bool retVal = false;

            try
            {
                if (value.Trim() == "1")
                    retVal = true;
                else
                    bool.TryParse(value, out retVal);
            }
            catch
            {
                retVal = false;
            }

            return retVal;
        }

        public static object ToObject(DataRow row)
        {
            dynamic obj = new DynamicDictionary();
            foreach (DataColumn c in row.Table.Columns)
            {
                obj.SetValue(c.ColumnName, row[c.ColumnName]);
                //obj.//

            }
            return (object)obj;
            //foreach (PropertyInfo prop in this.GetType().GetProperties())
            //{
            //    if (dr.Table.Columns.Contains(prop.Name))
            //    {
            //        if (dr[prop.Name].GetType() == typeof(DBNull))
            //        {
            //            prop.SetValue(this, null);
            //        }
            //        else
            //        {
            //            dynamic
            //            prop.SetValue(this, ChangeType(dr[prop.Name], prop.PropertyType));
            //            prop.SetValue(this, Convert.ChangeType(dr[prop.Name], prop.PropertyType));
            //        }

            //    }
            //}
        }

        public static List<DynamicDictionary> ToDictionaryArray(DataTable dt)
        {
            List<DynamicDictionary> rows = new List<DynamicDictionary>();
            DynamicDictionary row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new DynamicDictionary();
                foreach (DataColumn col in dt.Columns)
                {
                    row.SetValue(col.ColumnName.ToLower(), dr[col]);
                }
                rows.Add(row);
            }
            return rows;
        }

        public static DynamicDictionary ToDictionary(DataRow dr)
        {
            DynamicDictionary row = new DynamicDictionary();
            foreach (DataColumn col in dr.Table.Columns)
            {
                row.Add(col.ColumnName.ToLower(), dr[col]);
            }
            return row;
        }

        public static DynamicDictionary ToDynamicDictionary(dynamic d)
        {
            DynamicDictionary dic = new DynamicDictionary();
            //check for null
            if (d == null)
            {
                return null;
            }
            //
            IEnumerable<string>  propNames = Dynamitey.Dynamic.GetMemberNames(d);
            foreach(string prop in propNames)
            {
                dic.Add(prop, Dynamitey.Dynamic.InvokeGet(d, prop));
            }
            return dic;
        }

        public static List<DynamicDictionary> ToDynamicDictionaryList(IEnumerable<SqlMapper.DapperRow> rows)
        {
            List<DynamicDictionary> list = new List<DynamicDictionary>();
            foreach (SqlMapper.DapperRow dr in rows)
            {
                list.Add(ToDynamicDictionary(dr));
            }
            return list;
        }

        public static DynamicDictionary ToDynamicDictionary(Dapper.SqlMapper.DapperRow row)
        {
            DynamicDictionary dd = new DynamicDictionary();
            //check for null
            if (row == null)
            {
                return null;
            }
            IDictionary<string, object> dic = row.GetDictionary();
            foreach(KeyValuePair<string, object> item in dic)
            {
                dd.Add(item.Key, item.Value);
            }
            return dd;
        }

        public static TEnum ToEnum<TEnum>(object value)
        {
            TEnum val;
            string st = value == null ? Convert.ToInt32(default(TEnum)).ToString() : value.ToString();
            val = (TEnum)Enum.Parse(typeof(TEnum), st);
            return val;
        }

        public static DynamicDictionary ToDictionaryFromJson(string jsonString)
        {
            DynamicDictionary paraList = new DynamicDictionary();
            if (jsonString != string.Empty)
            {
                Object obj = JsonConvert.DeserializeObject(jsonString);
                JObject o = (JObject)obj;
                //JObject o = JObject.Parse(content);

                if (o.HasValues)
                {
                    JToken item = o.First;
                    do
                    {
                        paraList.Add(item.Path, o.GetValue(item.Path).ToString());
                        //Console.WriteLine(item.Path + " => " + item.ToString());
                        item = item.Next;
                    } while (item != null);
                }
            }
            return paraList;
        }

        public static List<DynamicDictionary> ToDictionaryListFromJson(string jsonString)
        {
            List<DynamicDictionary> paraList = new List<DynamicDictionary>();
            if (jsonString != string.Empty)
            {
                Object obj = JsonConvert.DeserializeObject(jsonString);
                if(obj is JArray)
                {
                    JArray arr = ((JArray)obj);
                    for (int i = 0, len = arr.Count; i < len; i++)//JObject o in arr)
                    {
                        DynamicDictionary dic = new DynamicDictionary();
                        JObject o = (JObject)arr[i];

                        if (o.HasValues)
                        {
                            JToken item = o.First;
                            do
                            {
                                string key = ((Newtonsoft.Json.Linq.JProperty)item).Name;
                                //dic.Add(item.Path, o.Value<object>(item.Path).ToString());
                                dic.Add(key, o.GetValue(key).ToString());
                                //Console.WriteLine(item.Path + " => " + item.ToString());
                                item = item.Next;
                            } while (item != null);
                        }
                        paraList.Add(dic);
                    }


                }                
            }
            return paraList;
        }
        public static DynamicDictionary ToDictionaryFormKeyValue(string formData)
        {
            DynamicDictionary paraList = new DynamicDictionary();
            if (formData != string.Empty)
            {
                string[] items = formData.Split(new char[] { '&' });
                foreach (string item in items)
                {
                    string[] arr = item.Split(new char[] { '=' }, 2);
                    if (arr.Length == 2)
                    {
                        paraList.Add(arr[0], arr[1]);
                    }

                }
            }
            return paraList;
        }

        //public static ResponseModel ToResponseModel(JObject jObj)
        //{
        //    ResponseModel resp = new ResponseModel(false, null);
        //    foreach (JToken token in jObj.Children())
        //    {
        //        if (token is JProperty)
        //        {
        //            var prop = token as JProperty;
        //            switch (prop.Name.Trim().ToLower())
        //            {
        //                case "success":
        //                    if (prop.Value.ToString().ToLower().Trim() == "true")
        //                        resp.success = true;
        //                    break;
        //                case "message":
        //                    resp.message = prop.Value.ToString();
        //                    break;
        //                case "data":
        //                    resp.data = prop.Value;
        //                    break;
        //            }
        //        }
        //    }
        //    return resp;
        //}
    }
}
