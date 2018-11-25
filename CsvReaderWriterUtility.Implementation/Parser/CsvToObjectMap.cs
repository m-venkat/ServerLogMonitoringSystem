using System;
using System.Linq.Expressions;

namespace CsvReadWriteUtility.Parser
{

    /// <summary>
    /// Internal class to store the object property to csv column mapping.  This structure is used by
    /// <see cref="ICsvToObjectMapper{T}"/> class to add the mapping 
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    internal class CsvToObjectMap<T> : ICsvToObjectMap<T>
    {
        /// <summary>
        /// Domain Object Property name as string
        /// </summary>
        public string PropertyName { get;  set; }
        public String CsvColumnName { get;  set; }
    }
}