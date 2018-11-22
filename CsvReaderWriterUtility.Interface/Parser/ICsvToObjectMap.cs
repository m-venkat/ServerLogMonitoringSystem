using System;
using System.Linq.Expressions;

namespace CsvReadWriteUtility.Parser
{
    public interface ICsvToObjectMap<T>
    {
        MemberExpression Property { get; set; }
        String CsvColumnName { get; set; }
    }
}