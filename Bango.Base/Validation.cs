using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Base
{
    public class Validation
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val">STRING VALUE THAT HAS TO BE CHECKED.</param>
        /// <param name="NumberStyle">NUMBER STYLE</param>
        /// <returns></returns>
        public static bool IsNumeric(string val, System.Globalization.NumberStyles NumberStyle)
        {
            Double result;
            bool retVal = false;
            try
            {
                retVal = Double.TryParse(val, NumberStyle, System.Globalization.CultureInfo.CurrentCulture, out result);
            }
            catch
            {
                //
            }
            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val">STRING VALUE THAT HAS TO BE CHECKED.</param>
        /// <param name="NumberStyle">NUMBER STYLE</param>
        /// <returns></returns>
        public static bool IsNumeric(string val)
        {
            Double result;
            bool retVal = false;
            try
            {
                retVal = Double.TryParse(val, out result);
            }
            catch
            {
                //
            }
            return retVal;
        }

        /// <summary>
        /// Check if the variable has Null value or not.
        /// </summary>
        /// <param name="Value">The value of which DBNull is being checkd</param>
        /// <remarks>Boolean value specifying whether the passed value is Null or not.</returns>
        public static bool IsDBNull(object Value)
        {

            if (Value == null)
                return true;


            if (Value is System.DBNull)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if the variable has Null value or not . If the passed value is DB NULL type then returns a default value
        /// </summary>
        /// <param name="Value">The value of which DBNull is being checkd</param>
        /// <param name="DataType">DataType for returing the default value if null exists</param>
        /// <returns></returns>
        public static object IsDBNull(object Value, DbType DataType)
        {

            if (Value is System.DBNull)
            {
                switch (DataType)
                {
                    case DbType.String:
                        Value = "";
                        break;
                    case DbType.Int32:
                    case DbType.Int16:
                    case DbType.Int64:
                    case DbType.UInt16:
                    case DbType.UInt32:
                    case DbType.UInt64:
                    case DbType.Byte:
                    case DbType.Currency:
                    case DbType.Decimal:
                    case DbType.Double:
                        Value = 0;
                        break;
                    case DbType.Boolean:
                        Value = false;
                        break;
                    case DbType.Date:
                    case DbType.DateTime:
                        Value = "";
                        break;
                }
            }
            return Value;
        }

        public static bool CheckIpAddress(string ipAddress)
        {
            bool Status = true;

            // IF THERE IS NO IP ADDRESS THEN IT IS SUPPOSE TO LOCALHOST.
            if (ipAddress.Trim().Length == 0)
            {
                return Status;
            }

            // SPLITING THE IP ADDRESS FOR VALIDATION.
            string[] splitIp = ipAddress.Split('.');

            // CHECKING THE LENGTH OF IP ADDRESS.
            // THE DEFAULT LENGHT IS 4. AS 255.255.255.255
            if (splitIp.Length == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    // CHECKING EACH IP ADDRESS.
                    if (Convert.ToInt32(splitIp[i]) > 255 || Convert.ToInt32(splitIp[i]) < 0)
                    {
                        Status = false;
                        break;
                    }
                }
            }
            else
            {
                Status = false;
            }

            /* TODO
             * if (Status == false)
                MessageBox.Show("Invalid Ip Address");*/

            return Status;
        }

        public static bool IsDateTime(string Val)
        {
            DateTime dt = new DateTime();
            try
            {
                return DateTime.TryParse(Val, out dt);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsNumericType(Type type)
        {
            return type.IsValueType;
            //switch (Type.GetTypeCode(type))
            //{
            //    case TypeCode.Byte:
            //    case TypeCode.SByte:
            //    case TypeCode.UInt16:
            //    case TypeCode.UInt32:
            //    case TypeCode.UInt64:
            //    case TypeCode.Int16:
            //    case TypeCode.Int32:
            //    case TypeCode.Int64:
            //    case TypeCode.Decimal:
            //    case TypeCode.Double:
            //    case TypeCode.Single:
            //        return true;
            //    default:
            //        return false;
            //}
        }
    }
}
