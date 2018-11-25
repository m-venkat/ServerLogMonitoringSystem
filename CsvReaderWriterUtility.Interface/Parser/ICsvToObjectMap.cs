using System;
using System.Linq.Expressions;

namespace CsvReadWriteUtility.Parser
{
    public interface ICsvToObjectMap<T>
    {
        string PropertyName { get; set; }
        String CsvColumnName { get; set; }
    }
}