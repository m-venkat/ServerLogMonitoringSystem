using System;
using System.Linq.Expressions;

namespace CsvReadWriteUtility.Parser
{
    public class CsvToObjectMap<T> : ICsvToObjectMap<T>
    {
        public MemberExpression Property { get; set; }
        public String CsvColumnName { get; set; }
    }
}