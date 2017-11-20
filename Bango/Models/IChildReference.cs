using System;
namespace Bango.Models
{
    public interface IChildReference
    {
        System.Collections.Generic.List<string> ColumnNames { get; set; }
        Type DataType { get; set; }
        string TableName { get; set; }
        string ToString();
    }
}
