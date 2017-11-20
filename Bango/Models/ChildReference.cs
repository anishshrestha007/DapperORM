using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bango.Models
{
    public class ChildReference : IChildReference
    {
        public string TableName { get; set; }
        public List<string> ColumnNames { get; set; }
        public Type DataType { get; set; }
        public static string ParameterName = ":master_id";
        public ChildReference()
        {
            ColumnNames = new List<string>();
            DataType = typeof(string);
        }

        public ChildReference(string tableName, string columnName, Type dataType)
            : this()
        {
            TableName = tableName;
            ColumnNames.Add(columnName);
            DataType = dataType;
        }
        public ChildReference(string tableName, string columnName)
            : this(tableName, columnName, typeof(Int32))
        {
        }

        public ChildReference(string tableName, string[] columnNames, Type dataType)
            : this()
        {
            TableName = tableName;
            ColumnNames.AddRange(columnNames);
            DataType = dataType;
        }
        public ChildReference(string tableName, string[] columnNames)
            : this(tableName, columnNames, typeof(Int32))
        {

        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT sum(1) as cnt FROM  {0} WHERE ", TableName);
            string or = string.Empty;
            foreach (string item in ColumnNames)
            {
                sb.AppendFormat(" {2} {0} = {1} ", item, ParameterName, or);
                or = "OR";
            }
            return sb.ToString();
        }

    }
}
