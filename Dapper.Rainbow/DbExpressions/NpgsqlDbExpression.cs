using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dapper.Rainbow
{
    public class NpgsqlDbExpression : Dapper.Rainbow.IDbExpression

    {
        string _dateFormat = string.Empty;
        string _dateTimeFormat = string.Empty;
        string _timeFormat = string.Empty;
        public NpgsqlDbExpression()
        {
            _dateFormat = "yyyy-MM-dd";
            _dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            _timeFormat = "1900-01-01 HH:mm:ss";
        }
        public virtual string IfNull(string columnOrExpression, string value)
        {
            return string.Format(" COALESCE({0}, '{1}')", columnOrExpression, value);
        }

        public virtual string IfNull(string columnOrExpression, int value)
        {
            return string.Format(" COALESCE({0}, {1})", columnOrExpression, value);
        }

        public virtual string IfNull(string columnOrExpression, bool value = false)
        {
            return string.Format(" COALESCE({0}, {1})", columnOrExpression, value);
        }

        public virtual string RowNumber(string columnAlias = null)
        {
            if (columnAlias == null)
                columnAlias = "rnum";
            return string.Format("row_number() OVER () as {0}", columnAlias);
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
        public string ToDbTime(string dt)
        {
            return ToDateTime(dt).ToString(_timeFormat);
        }
        public string ToDbTime(DateTime dt)
        {
            return dt.ToString(_timeFormat);
        }
        public string ToDbDate(string dt)
        {
            return ToDateTime(dt).ToString(_dateFormat);
        }
        public string ToDbDateTime(string dt)
        {
            return ToDateTime(dt).ToString(_dateTimeFormat);
        }
        public string ToDbDate(DateTime dt)
        {
            return dt.ToString(_dateFormat);
        }
        public string ToDbDateTime(DateTime dt)
        {
            return dt.ToString(_dateTimeFormat);
        }
        public string DateTruncate(string expressionOrField)
        {
            return string.Format("{0}::date", expressionOrField);
        }

        public string TimeOnlyTruncate(string expressionOrField)
        {
            return string.Format("{0}::Time", expressionOrField);
        }
    }
}
