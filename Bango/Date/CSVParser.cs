using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Bango.Date
{
    class CSVParser
    {
        private bool _IsFirstRowHeader = true;
        private int _totalRowsToParse = 0;
        private Encoding _encode;    // encoding for reading japanese characters

        #region "Constructors and Finalizers"

        public CSVParser()
        {
            _encode = System.Text.Encoding.GetEncoding("Shift-JIS");
        }

        /// <summary>
        /// CSVParser constructor
        /// </summary>
        /// <param name="encode">Encoding using which the csv file is to be read.</param>
        public CSVParser(Encoding encode)
        {
            _encode = encode;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets no of rows to be parsed by the CSV parse. Default is 0 which means all rows.
        /// </summary>
        public int TotalRowsToParse
        {
            get { return _totalRowsToParse; }
            set { _totalRowsToParse = value; }
        }

        /// <summary>
        /// Gets or sets the econding of the csv file to be read.
        /// </summary>
        public Encoding Encode
        {
            get { return _encode; }
            set { _encode = value; }
        }

        /// <summary>
        /// Gets or Sets if the first row of the CSV data is Header row or not.
        /// </summary>
        public bool IsFirstRowHeader
        {
            get { return _IsFirstRowHeader; }
            set { _IsFirstRowHeader = value; }
        }

        #endregion

        #region "Methods"

        /// <summary>
        /// Parses the CSV data,
        /// </summary>
        /// <param name="inputString">The CSV data that needs to be parsed.</param>
        /// <returns>DataTable with data data CSV data</returns>
        public DataTable ParseCSV(string inputString)
        {
            DataTable dt = new DataTable();

            // declare the Regular Expression that will match versus the input string
            Regex re = new Regex("((?<field>[^\",\\r\\n]+)|\"(?<field>([^\"]|\"\")+)\")(,|(?<rowbreak>\\r\\n|\\n|$))");

            ArrayList colArray = new ArrayList();
            ArrayList rowArray = new ArrayList();

            int colCount = 0;
            int maxColCount = 0;
            string rowbreak = "";
            string field = "";

            MatchCollection mc = re.Matches(inputString);

            foreach (Match m in mc)
            {

                // retrieve the field and replace two double-quotes with a single double-quote
                field = m.Result("${field}").Replace("\"\"", "\"");

                rowbreak = m.Result("${rowbreak}");

                //By Shakti on 2008/11/26 
                //REASON : code commeted to allow fields with 0 length.
                //EDIT Start
                if (field.Length >= 0)
                {
                    colArray.Add(field);
                    colCount++;
                }
                //EDIT END
                if (rowbreak.Length > 0)
                {

                    // add the column array to the row Array List
                    rowArray.Add(colArray.ToArray());

                    // create a new Array List to hold the field values
                    colArray = new ArrayList();

                    if (colCount > maxColCount)
                        maxColCount = colCount;

                    colCount = 0;
                }
            }

            if (rowbreak.Length == 0)
            {
                // this is executed when the last line doesn't
                // end with a line break
                rowArray.Add(colArray.ToArray());
                if (colCount > maxColCount)
                    maxColCount = colCount;
            }

            // convert the row Array List into an Array object for easier access
            Array ra = rowArray.ToArray();

            // return null if no data exists the file
            if (ra.Length == 0)
                return null;

            // header row
            Array headerArray;
            //if(_IsFirstRowHeader)
            //{
            headerArray = (Array)ra.GetValue(0);
            //}

            // create the columns for the table
            for (int i = 0; i < maxColCount; i++)
            {
                //add header text if first row is the Header row
                if (_IsFirstRowHeader)
                {
                    if (i < headerArray.Length)
                    {
                        dt.Columns.Add(headerArray.GetValue(i).ToString().Trim());
                        continue;
                    }
                }

                dt.Columns.Add(String.Format("col{0:000}", i));
            }

            //if first row is the header row then start adding the data from second row
            int r = _IsFirstRowHeader ? 1 : 0;

            for (; r < ra.Length; r++)
            {

                // create a new DataRow
                DataRow dr = dt.NewRow();

                // convert the column Array List into an Array object for easier access
                Array ca = (Array)(ra.GetValue(r));


                // add each field into the new DataRow
                for (int j = 0; j < ca.Length; j++)
                    dr[j] = ca.GetValue(j);

                // add the new DataRow to the DataTable
                dt.Rows.Add(dr);

                if (_totalRowsToParse > 0 && r >= _totalRowsToParse)
                    break;
            }

            // in case no data was parsed, create a single column
            if (dt.Columns.Count == 0)
                return null;
            //dt.Columns.Add("NoData");

            return dt;
        }

        /// <summary>
        /// Parses the CSV file stored in the local system.
        /// </summary>
        /// <param name="path">The local file path.</param>
        /// <returns>DataTable with data data CSV data</returns>
        public DataTable ParseCSVFile(string path)
        {
            string inputString = "";
            // check that the file exists before opening it
            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path, _encode);
                inputString = sr.ReadToEnd();
                sr.Close();

            }

            if (inputString.Trim().Length > 0)
                return ParseCSV(inputString);
            else
                return null;
        }

        /// <summary>
        /// Parses the data from CSV file located in remote.
        /// </summary>
        /// <param name="URL">The URL that points to an CSV file.</param>
        /// <returns>DataTable with data data CSV data</returns>
        public DataTable ParseCSV_URL(string URL)
        {
            try
            {
                WebClient client = new WebClient();

                // opening stream which will read data from URL
                Stream WebStream = client.OpenRead(URL);

                //Parse the stream and return the DataTable
                return ParseCSV_Stream(WebStream);

            }
            catch (WebException)
            {
                //System.Windows.Forms.MessageBox.Show("INVALID URL.");
                //web exception (normally. file not found)
            }
            catch //(Exception)
            {
                //
            }

            return null;
        }

        /// <summary>
        /// Parses data any stream that is alreay opened.
        /// </summary>
        /// <param name="pStream">The stream from where the data will be parsed.</param>
        /// <returns>DataTable with data data CSV data</returns>
        public DataTable ParseCSV_Stream(Stream pStream)
        {
            string inputString = "";

            StreamReader sr = new StreamReader(pStream, _encode);
            inputString = sr.ReadToEnd();
            sr.Close();

            if (inputString.Trim().Length > 0)
                return ParseCSV(inputString);
            else
                return null;

        }
        #endregion
    }
}