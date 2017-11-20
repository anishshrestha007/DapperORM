//using CsvFile;
//using Pmis.Common;
using Bango;
using Bango.Base;
using Bango.Base.Csv;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace Bango.Date
{
    public static class DateConvertor
    {
        #region Member variable declaration
        private static Hashtable _htNepIndx = new Hashtable();
        private static Hashtable _htEngIndx = new Hashtable();

        private static List<ConversionData> _lstConverstionInfo = new List<ConversionData>();
        private static DataTable _dtConversionData = null;


        private static StringBuilder _conversionData = new StringBuilder();

        private static int _minAllowedNepaliYear = 0;
        private static int _maxAllowedNepaliYear = 0;
        #endregion

        #region Constructors and Finalizers
        static DateConvertor()
        {
            //LoadConverstionData(year);
        }
        #endregion

        #region Class struct enums
        private class ConversionData
        {
            public string NepaliDate = string.Empty;
            public DateTime EnglishDate = new DateTime();
            public int DaysInNepaliMonth = 0;
        }
        #endregion

        #region Properties


        public static int MinAllowedNepaliYear
        {
            get { return DateConvertor._minAllowedNepaliYear; }
            set { DateConvertor._minAllowedNepaliYear = value; }
        }


        public static int MaxAllowedNepaliYear
        {
            get { return DateConvertor._maxAllowedNepaliYear; }
            set { DateConvertor._maxAllowedNepaliYear = value; }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Converts NepaliDate into English Date.
        /// DateFormat must be yyyy/MM/dd
        /// </summary>
        /// <param name="nepaliDate"></param>
        /// <returns></returns>
        public static DateTime GetEnglishDate(string nepaliDate)
        {
            //search the nepali year and month in the hashTable.
            string searchString = nepaliDate.Substring(0, 7);
            int year = Int32.Parse(searchString.Substring(0, 4));
            //_conversionData.Clear();
            LoadConverstionData(year, true);

            DateTime convertedDate = new DateTime();
            if (_htNepIndx.ContainsKey(searchString))
            {
                int rowIndx = (int)_htNepIndx[searchString];

                DateTime engDate = Convert.ToDateTime(_dtConversionData.Rows[rowIndx]["EnglishDate"]);//,"yyyy/MM/dd",System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat);

                convertedDate = engDate.AddDays(GetDay(nepaliDate) - 1);

                return convertedDate;

            }
            throw new Exception("Date not available for conversion");
        }
        public static String ToNepaliDate(string englishDate)
        {
            if (englishDate.Trim().Length == 0)
            {
                return string.Empty;
            }
            return ToNepaliDate(Convert.ToDateTime(englishDate));
        }
        public static String ToNepaliDate(DateTime englishDate)
        {
            NepaliDate dt = null;
            try
            {
                dt = GetNepaliDate(englishDate);
                if (dt != null)
                {
                    return dt.ToString();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static DateTime? ToEnglishDate(string nepaliDate)
        {
            DateTime dt;
            try
            {
                dt = GetEnglishDate(nepaliDate);
                if (dt != null)
                {
                    return dt;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static NepaliDate GetNepaliDate(DateTime englishDate)
        {
            //search the nepali year and month in the hashTable.
            string searchString = englishDate.ToString("yyyy/MM").Replace("-", "/");
            int year = Int32.Parse(searchString.Substring(0, 4));
            LoadConverstionData(year, false);
            string convertedDate = string.Empty;
            if (_htEngIndx.ContainsKey(searchString))
            {
                int rowIndx = (int)_htEngIndx[searchString];

                //Convert the english date to nepali
                DateTime tmpEngDate = Convert.ToDateTime(_dtConversionData.Rows[rowIndx]["EnglishDate"]);


                int dayOfNepaliMnth = 0;
                int daysInNepaliMnth = 0;


                if (englishDate.Date >= tmpEngDate.Date)
                {
                    dayOfNepaliMnth = englishDate.Day - tmpEngDate.Day + 1;
                    convertedDate = _dtConversionData.Rows[rowIndx]["NepaliDate"].ToString();
                }
                else
                {
                    //get the no of days in previous nepali month
                    daysInNepaliMnth = Convert.ToInt32(_dtConversionData.Rows[rowIndx - 1]["DaysInNepaliMonth"]);
                    convertedDate = _dtConversionData.Rows[rowIndx - 1]["NepaliDate"].ToString();

                    dayOfNepaliMnth = daysInNepaliMnth - (tmpEngDate.Day - englishDate.Day - 1);
                }

                //format the date to be given to as return.
                convertedDate = string.Format("{0}/{1:00}", convertedDate.Substring(0, 7), dayOfNepaliMnth);

                return new NepaliDate(convertedDate);
            }
            throw new Exception("Date not available for conversion");

        }

        /// <summary>
        /// Provides the No of days in the given Nepali month of specific year.
        /// </summary>
        /// <param name="year">Year</param>
        /// <param name="month">Month</param>
        /// <returns>Return the no days in Nepali month if exists else return 0.</returns>
        public static int GetDaysInNepaliMoth(int year, int month)
        {
            //search the nepali year and month in the hashTable.
            string searchString = string.Format("{00:0000}/{1:00}", year, month);
            if (_htNepIndx.ContainsKey(searchString))
            {
                int rowIndx = (int)_htNepIndx[searchString];

                return Convert.ToInt32(_dtConversionData.Rows[rowIndx]["DaysInNepaliMonth"]);
            }
            else
            {
                return 0;
            }
        }

        private static int GetDay(string date)
        {
            return Convert.ToInt32(date.Substring(8, 2));
        }
        // Added by jayan  June 22 2009
        public static String GetFiscalYear(String npdate)  //Function for returning fiscal year from a given nepali date
        {
            NepaliDate date1 = new NepaliDate(npdate);
            String fyear;
            int mon = date1.Month;
            int yea = date1.Year;
            int lessyear = (yea - 1);
            int greatyear = (yea + 1);
            String fiscalstartdate = yea.ToString() + "/04/01";
            int x = npdate.CompareTo(fiscalstartdate);
            if (x == 1)
            {
                fyear = yea.ToString() + "/" + greatyear.ToString(); ;
            }
            else
            {
                fyear = lessyear.ToString() + "/" + yea.ToString();
            }
            return fyear;

        }

        private static void LoadConversionDataIntoString(int year, bool yeartype)// yeartype true= nep, false = eng
        {
            int prevyear = year - 1,
                nextyear = year + 1;
            _conversionData.AppendLine("NepaliDate,EnglishDate,DaysInNepaliMonth");
            string filepath, prevYearFilepath, nextYearFilepath;

            if (yeartype)
            {
                prevYearFilepath = FileBox.GetBsDateFile(prevyear);
                filepath = FileBox.GetBsDateFile(year);
                nextYearFilepath = FileBox.GetBsDateFile(nextyear);
            }
            else
            {
                prevYearFilepath = FileBox.GetAdDateFile(prevyear);
                filepath = FileBox.GetAdDateFile(year);
                nextYearFilepath = FileBox.GetAdDateFile(nextyear);
            }

            List<string> columnNames = new List<string>(){
                                                "NepaliDate",
                                                "EnglishDate",
                                                "DaysInMonth"
                                            };


            //load the both previous year and next year data
            CsvFileReader prevfile = new CsvFileReader(prevYearFilepath);
            Dictionary<int, string> resultprev = prevfile.ReadFile(columnNames);
            prevfile.Dispose();
            foreach (KeyValuePair<int, string> pair in resultprev)
            {
                _conversionData.AppendLine(pair.Value);
            }


            CsvFileReader file = new CsvFileReader(filepath);
            Dictionary<int, string> result = file.ReadFile(columnNames);
            file.Dispose();
            foreach (KeyValuePair<int, string> pair in result)
            {
                _conversionData.AppendLine(pair.Value);
            }


            CsvFileReader nextfile = new CsvFileReader(nextYearFilepath);
            Dictionary<int, string> resultnext = nextfile.ReadFile(columnNames);
            nextfile.Dispose();
            foreach (KeyValuePair<int, string> pair in resultnext)
            {
                _conversionData.AppendLine(pair.Value);
            }


        }

        public static void LoadConverstionData(int year, bool yeartype)
        {

            //Load the conversion info into the datatable
            _conversionData.Clear();
            _htNepIndx.Clear();
            _htEngIndx.Clear();
            LoadConversionDataIntoString(year, yeartype);
            CSVParser prsr = new CSVParser();
            prsr.IsFirstRowHeader = true;
            _dtConversionData = prsr.ParseCSV(_conversionData.ToString());

            //Now load the conversion info into hastables too.
            int rowIndx = 0;   //its the corresponding index of the row in which the entire conversion information exists.
            int tmpYear = 0;
            foreach (DataRow row in _dtConversionData.Rows)
            {
                if (!_htNepIndx.ContainsKey(row["NepaliDate"].ToString().Substring(0, 7)))
                {
                    _htNepIndx.Add(row["NepaliDate"].ToString().Substring(0, 7), rowIndx);
                    _htEngIndx.Add(row["EnglishDate"].ToString().Substring(0, 7), rowIndx);

                    tmpYear = Conversion.ToInt32(row["NepaliDate"].ToString().Substring(0, 4));
                    if (tmpYear > _maxAllowedNepaliYear)
                        _maxAllowedNepaliYear = tmpYear;

                    if (tmpYear < _minAllowedNepaliYear)
                        _minAllowedNepaliYear = tmpYear;
                    else if (_minAllowedNepaliYear == 0)
                        _minAllowedNepaliYear = _maxAllowedNepaliYear;
                }

                rowIndx = rowIndx + 1;
            }
        }

        /// <summary>
        /// Check if the year provided exists in the DB and returns boolean status.
        /// </summary>
        /// <param name="year"></param>
        /// <returns>Returns true if valild else false.</returns>
        public static bool IsValidYear(int year, bool yeartype)
        {
            //currently the checking is done by assuming that the if the data of particular year exists 
            //then all 12 month data will be there.
            LoadConverstionData(year, yeartype);
            if (_htNepIndx.ContainsKey(string.Format("{0}/01", year)))
                return true;
            else
                return false;
        }
        #endregion
    }
}