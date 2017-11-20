using Bango.Base.List;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Report
{

    /*
        column-x    -> column-header
        column-y    -> row-header
        value field
        group-function
            -> count, sum, average
    */
    //count
    //sum
    //
    public enum PivotGroupFunction
    {
        None,
        Sum,
        Count,
        Average,
        Max,
        Min
    }
    public enum PivotSortMode
    {
        Ascending,
        Descending
    }
    public class PivotTableGenerator
    {
        public class PivotValue
        {
            public double Sum { get; set; } = 0;
            public int Count { get; set; } = 0;
            public double Average { get; set; } = 0;
            public double Max { get; set; } = 0;
            public double Min { get; set; } = 0;
        }

        public class PivotData
        {
            public string ValueX { get; set; }
            public string ValueY { get; set; }
            public PivotValue Value { get; set; } = new PivotValue();
        }

        public class PivotColumn
        {
            public string Name1 { get; set; }
            public string Name2 { get; set; }
            public bool CalculateSummary { get; set; } = true;
            public PivotGroupFunction SummaryGroupFunction { get; set; } = PivotGroupFunction.Sum;
            public string SummaryLabel { get; set; } = "total";
        }
        public bool IgnoreNullValues { get; set; } = true;
        public PivotColumn ColumnX { get; set; } = new PivotColumn();
        public PivotColumn ColumnY { get; set; } = new PivotColumn();

        public string ColumnValue { get; set; }
        public double GrandTotal { get; set; } = 0;
        public double ArrTotal { get; set; } = 0;
        public double DepTotal { get; set; } = 0;
        public PivotGroupFunction ValueGroupFunction { get; set; } = PivotGroupFunction.None;
        //public PivotGroupFunction ColumnXGroupFunction { get; set; } = PivotGroupFunction.Sum;
        //public PivotGroupFunction ColumnYGroupFunction { get; set; } = PivotGroupFunction.Sum;

        List<string> xKeys = new List<string>();
        List<string> yKeys = new List<string>();
        private Dictionary<string, PivotValue> SummaryOfX = new Dictionary<string, PivotValue>();
        private Dictionary<string, PivotValue> SummaryOfY = new Dictionary<string, PivotValue>();
        private List<PivotData> SummaryOfData = new List<PivotData>();
        private DataTable FinalPivotDataTable = new DataTable();
        private List<DynamicDictionary> FinalPivotDictionary = new List<DynamicDictionary>();
        public DataTable PreparePivotTable(DataTable dataSource, string colX, string colY, string colValue, PivotGroupFunction valueGroupFn = PivotGroupFunction.None)
        {
            ColumnX.Name1 = colX;
            ColumnY.Name1 = colY;
            ColumnValue = colValue;
            ValueGroupFunction = valueGroupFn;
            return PreparePivotTable(dataSource);
        }

        public DataTable PreparePivotTable(DataTable dataSource)
        {
            //DataSource = DataSource.Select().OrderBy("a,b");
            //DataTable sortedData = SortData(dataSource, ColumnX.Name, ColumnY.Name);
            xKeys = GetDistinctValues(dataSource, ColumnX.Name1, ColumnY.Name2);
            yKeys = GetDistinctValues(dataSource, ColumnY.Name1, ColumnY.Name2);

            FinalPivotDataTable = PreparePivotTable(yKeys, xKeys);
            FinalPivotDataTable = PivotTheData(dataSource);
            return FinalPivotDataTable;
        }
        public List<DynamicDictionary> PreparePivotTable(List<DynamicDictionary> dataSource)
        {
            //DataSource = DataSource.Select().OrderBy("a,b");
            //DataTable sortedData = SortData(dataSource, ColumnX.Name, ColumnY.Name);
            xKeys = GetDistinctValues(dataSource, ColumnX.Name1, string.Empty);
            yKeys = GetDistinctValues(dataSource, ColumnY.Name1, ColumnY.Name2);

            FinalPivotDictionary = PreparePivotDictionary(yKeys, xKeys);
            FinalPivotDictionary = PivotTheData(dataSource);
            return FinalPivotDictionary;
        }
        private string GetStringValue(DataRow dr, string columnName1, string columnName2)
        {
            return Bango.Base.Conversion.ToString(dr, columnName1);
        }
        private double GetDoubleValue(DataRow dr, string columnName)
        {
            return Bango.Base.Conversion.ToDouble(dr, columnName);
        }
        private PivotData GetPivotData(string valX, string valY)
        {
            IEnumerable<PivotData> flt = SummaryOfData.Where(d => d.ValueX == valX && d.ValueY == valY);
            if (flt.Count() > 0)
            {
                return flt.ElementAt(0);
            }
            PivotData pd = new PivotData() { ValueX = valX, ValueY = valY };
            SummaryOfData.Add(pd);
            return pd;
        }

        public DataTable PivotTheData(DataTable sourceData)
        {
            try
            {
                //manage or prepare distict of the values
                double val = 0;
                string valx = string.Empty, valy = string.Empty;
                SummaryOfData.Clear();
                for (int i = 0, len = sourceData.Rows.Count; i < len; i++)
                {
                    valx = GetStringValue(sourceData.Rows[i], ColumnX.Name1, ColumnY.Name2);
                    valy = GetStringValue(sourceData.Rows[i], ColumnY.Name1, ColumnY.Name2);
                    val = GetDoubleValue(sourceData.Rows[i], ColumnValue);

                    PivotData pd = GetPivotData(valx, valy);
                    pd.Value = CalculateSummary(pd.Value, val);
                }
                DataTable dt = FinalPivotDataTable;
                double tot = 0;
                //now put the data in DataTable
                foreach (string y in yKeys)
                {
                    if (y.Trim().Length == 0)
                        continue;
                    IEnumerable<PivotData> yRows = SummaryOfData.Where(d => d.ValueY == y);
                    DataRow dr = dt.NewRow();
                    dr[ColumnY.Name1] = y;
                    PivotValue pv_y = SummaryOfY[y];
                    foreach (PivotData pd in yRows)
                    {
                        val = GetCellData(pd.Value, ValueGroupFunction);
                        dr[pd.ValueX] = val;
                        //calculate summary of Y
                        pv_y = CalculateSummary(pv_y, val);
                    }
                    tot = GetCellData(pv_y, ColumnY.SummaryGroupFunction);
                    dr[ColumnY.SummaryLabel] = tot;
                    dt.Rows.Add(dr);
                    //also calculating the grand total
                    GrandTotal += tot;
                }
                //calculate the summary of X
                if (ColumnY.CalculateSummary)
                {
                    DataRow dr = dt.NewRow();
                    dr[ColumnY.Name1] = ColumnX.SummaryLabel;
                    foreach (string x in xKeys)
                    {
                        IEnumerable<PivotData> xRows = SummaryOfData.Where(d => d.ValueX == x);
                        PivotValue pv_x = SummaryOfX[x];
                        foreach (PivotData pd in xRows)
                        {
                            pv_x = CalculateSummary(pv_x, GetCellData(pd.Value, ValueGroupFunction));
                        }
                        dr[x] = GetCellData(pv_x, ColumnX.SummaryGroupFunction);
                    }
                    if (dt.Columns.Contains(ColumnX.SummaryLabel))
                        dr[ColumnX.SummaryLabel] = GrandTotal;
                    dt.Rows.Add(dr);
                }

                return dt;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public List<DynamicDictionary> PivotTheData(List<DynamicDictionary> sourceData)
        {

            DynamicDictionary disc = sourceData[0];
            string mov = disc.GetValueAsString("mov").ToUpper().Trim();
            try
            {
                SummaryOfData.Clear();
                //manage or prepare distict of the values
                double val = 0;
                string valx = string.Empty, valy = string.Empty;
                for (int i = 0, len = sourceData.Count; i < len; i++)
                {
                    valx = sourceData[i].GetValueAsString(ColumnX.Name1);
                    valy = sourceData[i].GetValueAsString(ColumnY.Name2) + sourceData[i].GetValueAsString(ColumnY.Name1);
                    val = sourceData[i].GetValue<Double>(ColumnValue);

                    PivotData pd = GetPivotData(valx, valy);
                    pd.Value = CalculateSummary(pd.Value, val);
                }
                List<DynamicDictionary> dt = FinalPivotDictionary;
                double tot = 0;
                //now put the data in DataTable
                foreach (string y in yKeys)
                {
                    if (y.Trim().Length == 0)
                        continue;
                    IEnumerable<PivotData> yRows = SummaryOfData.Where(d => d.ValueY == y);
                    DynamicDictionary dr = new DynamicDictionary();
                    //prepare the row/record
                    dr[ColumnY.Name1] = y;
                    PivotValue pv_y = SummaryOfY[y];
                    foreach (PivotData pd in yRows)
                    {
                        val = GetCellData(pd.Value, ColumnY.SummaryGroupFunction);
                        dr[pd.ValueX] = val;
                        //calculate summary of Y
                        pv_y = CalculateSummary(pv_y, val);
                    }
                    foreach (string x in xKeys)
                    {
                        if (!dr.ContainsKey(x))
                        {
                            dr[x] = string.Empty;
                        }
                    }
                    tot = GetCellData(pv_y, ColumnY.SummaryGroupFunction);
                    dr[ColumnY.SummaryLabel] = tot;
                    //
                    dt.Add(dr);
                    //also calculating the grand total  
                    GrandTotal += tot;

                    //Calculate total for arr and dep
                    if (mov == "DEP")
                        DepTotal = GrandTotal;
                    else
                        ArrTotal = GrandTotal - DepTotal;
                }
                DepTotal = GrandTotal;
                //calculate the summary of X
                if (ColumnY.CalculateSummary)
                {
                    DynamicDictionary dr = new DynamicDictionary();
                    dr[ColumnY.Name1] = ColumnX.SummaryLabel;
                    foreach (string x in xKeys)
                    {
                        IEnumerable<PivotData> xRows = SummaryOfData.Where(d => d.ValueX == x);
                        PivotValue pv_x = SummaryOfX[x];
                        foreach (PivotData pd in xRows)
                        {
                            pv_x = CalculateSummary(pv_x, GetCellData(pd.Value, ColumnX.SummaryGroupFunction));
                        }
                        dr[x] = GetCellData(pv_x, ColumnX.SummaryGroupFunction);
                    }
                    if (ColumnX.CalculateSummary)
                    {
                        dr[ColumnX.SummaryLabel] = mov == "ARR" ? ArrTotal : DepTotal;
                    }
                    dt.Add(dr);
                }
                return dt;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        private PivotValue CalculateSummary(PivotValue pv, double val)
        {
            pv.Count++;
            pv.Sum += val;
            pv.Average = pv.Sum / pv.Count;
            pv.Max = pv.Max > val ? pv.Max : val;
            pv.Min = pv.Min < val ? pv.Min : val;
            return pv;
        }

        private void SetRowColumnSummary(PivotData pd)
        {
            //
        }
        public double GetCellData(PivotValue pv, PivotGroupFunction grpFn)
        {
            switch (grpFn)
            {
                case PivotGroupFunction.Count:
                    return pv.Count;
                case PivotGroupFunction.Sum:
                    return pv.Sum;
                case PivotGroupFunction.Max:
                    return pv.Max;
                case PivotGroupFunction.Min:
                    return pv.Min;
                case PivotGroupFunction.Average:
                case PivotGroupFunction.None:
                default:
                    return pv.Average;
            }
        }

        public DataTable PreparePivotTable(List<string> yKeys, List<string> xKeys)
        {
            DataTable dt = new DataTable();
            SummaryOfX.Clear();
            dt.Columns.Add(ColumnY.Name1, typeof(string));
            foreach (string k in xKeys)
            {
                dt.Columns.Add(k, typeof(double));
                SummaryOfX.Add(k, new PivotValue());
            }
            if (ColumnX.CalculateSummary)
            {
                dt.Columns.Add("total", typeof(double));
            }

            SummaryOfY.Clear();
            foreach (string y in yKeys)
            {
                //dt.Rows.Add(y);
                SummaryOfY.Add(y, new PivotValue());
            }

            return dt;
        }

        public List<DynamicDictionary> PreparePivotDictionary(List<string> yKeys, List<string> xKeys)
        {
            List<DynamicDictionary> dt = new List<DynamicDictionary>();
            SummaryOfX.Clear();
            //dt.Columns.Add(ColumnY.Name, typeof(string));
            foreach (string k in xKeys)
            {
                //dt.Columns.Add(k, typeof(double));
                SummaryOfX.Add(k, new PivotValue());
            }
            //if (ColumnX.CalculateSummary)
            //{
            //    dt.Columns.Add("Total", typeof(double));
            //}

            SummaryOfY.Clear();
            foreach (string y in yKeys)
            {
                //dt.Rows.Add(y);
                SummaryOfY.Add(y, new PivotValue());
            }
            return dt;
        }

        public DataTable SortData(DataTable dataSource, string columnX, string columnY)
        {
            dataSource.DefaultView.Sort = $"{columnY} asc, {columnX} asc";
            DataTable SortedData = dataSource.DefaultView.ToTable();
            return SortedData;
        }
        public List<DynamicDictionary> SortData(List<DynamicDictionary> dataSource, string columnX, string columnY)
        {
            //dataSource.DefaultView.Sort = $"{columnY} asc, {columnX} asc";
            //DynamicDictionary SortedData = dataSource.DefaultView.ToTable();
            return dataSource;
        }
        public List<string> GetDistinctValues(DataTable DataSource, string column1, string column2, bool sort = true)
        {
            List<string> arr = new List<string>();
            for (int i = 0, len = (int)DataSource?.Rows.Count; i < len; i++)
            {
                if (!arr.Contains(DataSource.Rows[i][column1].ToString()))
                {
                    arr.Add(DataSource.Rows[i][column1].ToString());
                }
            }
            arr.Sort();
            return arr;
        }
        public List<string> GetDistinctValues(List<DynamicDictionary> DataSource, string column1, string column2, bool sort = true)
        {
            List<string> arr = new List<string>();
            List<string> newArr = new List<string>();
            for (int i = 0, len = (int)DataSource?.Count; i < len; i++)
            {
                DynamicDictionary row = DataSource[i];
                string s = row.GetValueAsString(column2) + row.GetValueAsString(column1);

                if (!arr.Contains(s))
                {
                    arr.Add(s);
                }
            }
            arr.Sort();
            return arr;
        }
    }
}
