using Bango.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bango.Date
{
    public enum DateFormatTypes
    {
        yyyyMMdd = 0,
        ddMMyyyy,
        MMddyyyy,
        yyyyMM = 10
    }

    public class NepaliDate : IComparable<NepaliDate>
    {
        #region Member variables.
        private int _year = 0;
        private int _month = 0;
        private int _day = 0;
        #endregion

        #region Constructors & Finalizers.
        public NepaliDate()
        {
            //store the lowest nepali date available in the db. 
        }

        /// <summary>
        /// Converts the year month and day into date.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        public NepaliDate(int year, int month, int day)
        {
            _year = year;
            _month = month;
            _day = day;
        }

        /// <summary>
        /// Initializes the object by converting the nepali date passed as string.
        /// </summary>
        /// <param name="nepaliDate">Nepali date in YYYY/MM/DD format.</param>
        public NepaliDate(string nepaliDate)
        {
            ConvertStringToNepaliDate(nepaliDate, DateFormatTypes.yyyyMMdd);
        }
        #endregion

        #region Nested Enums, Structs, and Classes.
        #endregion

        #region Properties
        public static NepaliDate Today
        {
            get
            {
                return DateConvertor.GetNepaliDate(DateTime.Today);
            }
        }

        public int Year
        {
            get { return _year; }
            set { _year = value; }
        }

        public int Month
        {
            get { return _month; }
            set { _month = value; }
        }

        public int Day
        {
            get { return _day; }
            set { _day = value; }
        }

        #endregion

        #region Methods

        public NepaliDate AddDays(int days)
        {
            DateTime dt = DateConvertor.GetEnglishDate(this.ToString());
            dt = dt.AddDays(days);
            return DateConvertor.GetNepaliDate(dt);
        }

        public NepaliDate AddMonths(int months)
        {
            DateTime dt = DateConvertor.GetEnglishDate(this.ToString());
            dt = dt.AddMonths(months);
            return DateConvertor.GetNepaliDate(dt);
        }

        public NepaliDate AddYears(int years)
        {
            DateTime dt = DateConvertor.GetEnglishDate(this.ToString());
            dt = dt.AddYears(years);
            return DateConvertor.GetNepaliDate(dt);
        }
        /// <summary>
        /// Check if the given date is nepali date or not and returns to if a valid date.
        /// </summary>
        /// <param name="nepaliDate"></param>
        /// <param name="dateFormatType"></param>
        /// <returns>Returns true if valid date else false.</returns>
        public static bool IsValidDate(string nepaliDate, DateFormatTypes dateFormatType)
        {
            int tmp = 0;
            switch (dateFormatType)
            {
                case DateFormatTypes.yyyyMMdd:
                    //convert the date in YYYY/MM/DD format eg. 2065/03/05 to day, month and year.
                    if (nepaliDate.Trim().Length == 10)
                    {
                        tmp = Conversion.ToInt32(nepaliDate.Substring(0, 4));
                        int yr, mnth, days;
                        //extract year.
                        if (DateConvertor.IsValidYear(tmp, true))
                            yr = tmp;
                        else
                            return false;

                        //extract month:
                        tmp = Conversion.ToInt32(nepaliDate.Substring(5, 2));
                        if (DateConvertor.GetDaysInNepaliMoth(yr, tmp) > 0)
                            mnth = tmp;
                        else
                            return false;

                        //extract day:
                        tmp = Conversion.ToInt32(nepaliDate.Substring(8, 2));
                        if (tmp > 0 &&
                            tmp <= DateConvertor.GetDaysInNepaliMoth(yr, mnth))
                            days = tmp;
                        else
                            return false;

                        return true;
                    }
                    //throw new Exception("Date format must be YYYY/MM/DD. eg. 2065/03/05");                        
                    break;
                default:
                    throw new Exception("Other date formats not supported");
            }
            return false;
        }


        /// <summary>
        /// Check if the given date is nepali date or not and returns to if a valid date.
        /// It converts the date in yyyy/MM/dd format.
        /// </summary>
        /// <param name="nepaliDate"></param>
        /// <returns>Returns true if valid date else false.</returns>
        public static bool IsValidDate(string nepaliDate)
        {
            return IsValidDate(nepaliDate, DateFormatTypes.yyyyMMdd);
        }

        private bool ConvertStringToNepaliDate(string nepaliDate, DateFormatTypes dateFormatType)
        {
            int tmp = 0;
            switch (dateFormatType)
            {
                case DateFormatTypes.yyyyMMdd:
                    //convert the date in YYYY/MM/DD format eg. 2065/03/05 to day, month and year.
                    if (nepaliDate.Trim().Length == 10)
                    {
                        tmp = Conversion.ToInt32(nepaliDate.Substring(0, 4));

                        //extract year.
                        if (DateConvertor.IsValidYear(tmp, true))
                            _year = tmp;
                        else
                            throw new Exception("Date format must be YYYY/MM/DD. eg. 2065/03/05");

                        //extract month:
                        tmp = Conversion.ToInt32(nepaliDate.Substring(5, 2));
                        if (DateConvertor.GetDaysInNepaliMoth(_year, tmp) > 0)
                            _month = tmp;
                        else
                            throw new Exception("Date format must be YYYY/MM/DD. eg. 2065/03/05");

                        //extract day:
                        tmp = Conversion.ToInt32(nepaliDate.Substring(8, 2));
                        if (tmp > 0 &&
                            tmp <= DateConvertor.GetDaysInNepaliMoth(_year, _month))
                            _day = tmp;
                        else
                            _day = DateConvertor.GetDaysInNepaliMoth(_year, _month); //current month days... if not exis
                            //throw new Exception("Date format must be YYYY/MM/DD. eg. 2065/03/05");
                        return true;
                    }
                    else
                        throw new Exception("Date format must be YYYY/MM/DD. eg. 2065/03/05");
                    //break;
                default:
                    throw new Exception("Other date formats not supported");
            }
            //return false;
        }

        public override string ToString()
        {
            return ToString(DateFormatTypes.yyyyMMdd);
        }

        public string ToString(DateFormatTypes dateFormatType)
        {
            string format = string.Empty;

            switch (dateFormatType)
            {
                case DateFormatTypes.yyyyMMdd:
                    format = "{0:0000}/{1:00}/{2:00}";
                    break;
                case DateFormatTypes.ddMMyyyy:
                    format = "{2:00}/{1:00}/{0:0000}";
                    break;
                case DateFormatTypes.MMddyyyy:
                    format = "{1:00}/{2:00}/{0:0000}";
                    break;
                case DateFormatTypes.yyyyMM:
                    format = "{0:0000}/{1:00}";
                    break;
            }

            return string.Format(format, _year, _month, _day);
        }

        /// <summary>
        /// Returns the fiscal year in which the given nepali Date falls
        /// </summary>
        /// <param name="npdate"></param>
        /// <returns>Returns fiscal year if valid nepali date else returns empty string</returns>
        public static string GetFiscalYear(String npdate)  //Function for returning fiscal year from a given nepali date
        {
            NepaliDate date1 = null;
            try
            {
                date1 = new NepaliDate(npdate);
            }
            catch
            {
                return string.Empty;
            }

            String fyear;
            int mon = date1.Month;
            int yea = date1.Year;
            int lessyear = (yea - 1);
            int greatyear = (yea + 1);

            //2065/04/01
            if (mon >= 4)
                fyear = string.Format("{0}/{1}", yea, yea + 1);
            else
                fyear = string.Format("{0}/{1}", yea - 1, yea);

            //String fiscalstartdate = yea.ToString() + "/04/01";
            //int x = npdate.CompareTo(fiscalstartdate);
            //if (x == 1)
            //{
            //    fyear = yea.ToString() + "/" + greatyear.ToString(); ;
            //}
            //else
            //{
            //    fyear = lessyear.ToString() + "/" + yea.ToString();
            //}
            return fyear;

        }
        #endregion

        #region IComparable<NepaliDate> Members

        public int CompareTo(NepaliDate other)
        {
            DateTime engOther = DateConvertor.GetEnglishDate(other.ToString());

            DateTime engThis = DateConvertor.GetEnglishDate(this.ToString());
            return engThis.CompareTo(engOther);

        }

        #endregion
    }

    public class NepaliMonth
    {
        public int DaysInMonth { get; private set; }
        public int Year { get; private set; }
        public int Month { get; private set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public NepaliMonth(int year, int month)
        {
            Year = year;
            Month = month;
            calc();
        }

        private void calc()
        {
            DaysInMonth = DateConvertor.GetDaysInNepaliMoth(Year, Month);
            NepaliDate np = new NepaliDate(Year, Month, 1);
            StartDate = DateConvertor.GetEnglishDate(np.ToString());
            np.Day = DaysInMonth;
            EndDate = DateConvertor.GetEnglishDate(np.ToString());
        }

    }
}