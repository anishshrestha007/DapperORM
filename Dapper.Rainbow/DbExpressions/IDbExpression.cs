using System;
namespace Dapper.Rainbow
{
    public interface IDbExpression
    {
        string DateTruncate(string column);
        string IfNull(string column, int value);
        string IfNull(string column, string value);
        string IfNull(string column, Boolean value);
        string RowNumber(string columnAlias = null);
        string ToDbDate(DateTime dt);
        string ToDbDate(string dt);
        string ToDbTime(DateTime dt);
        string ToDbTime(string dt);
        string ToDbDateTime(DateTime dt);
        string ToDbDateTime(string dt);

        string TimeOnlyTruncate(string expressionOrField);
    }
}
