using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using CsvReadWriteUtility.Exceptions;


namespace CsvReadWriteUtility.Parser
{

    public class CsvToObjectMap<T> : ICsvToObjectMap<T>
    {
        public MemberExpression Property { get;  set; }
        public String CsvColumnName { get;  set; }
    }

    /// <summary>
    /// Class responsible for adding Mapping between object property and CSV column name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public  class CsvToObjectMapper<T> : ICsvToObjectMapper<T>
    {
        public Dictionary<string, ICsvToObjectMap<T>> ObjectToCsvMapping { get; private set; } = new Dictionary<string, ICsvToObjectMap<T>>();

        
        /// <summary>
        /// This is the Mapper method that maps Domain object properties with Csv file column.
        /// Instance of this class needs to be passed in to the constructor of <see cref="ICsvToObjectReader{T}"/>
        /// </summary>
        /// <typeparam name="TKey">Property name of domain object</typeparam>
        /// <param name="property">Domain object property</param>
        /// <param name="csvColumnName">Column Name that appears in CSV file</param>
        public void AddMap<TKey>( Expression<Func<T, TKey>> property, string csvColumnName)
        {
            if (property is null)
                throw new CsvReadWriteException($"property cannot be null", ErrorCodes.ParameterNull);
            if (csvColumnName is null)
                throw new CsvReadWriteException($"csvColumnName cannot be null", ErrorCodes.ParameterNull);
            
            MemberExpression selectedPropertyAsExpresion = ((MemberExpression)property.Body);
            var name = selectedPropertyAsExpresion.Member.Name;
            ObjectToCsvMapping.Remove(name);
            ObjectToCsvMapping.Add(name, new CsvToObjectMap<T>() { CsvColumnName = csvColumnName, Property = selectedPropertyAsExpresion });
        }

    }


}
