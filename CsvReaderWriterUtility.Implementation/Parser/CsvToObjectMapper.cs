using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CsvReadWriteUtility.Exceptions;


namespace CsvReadWriteUtility.Parser
{
    /// <summary>
    /// Class responsible for specifying mapping between domain object selectedProperty and CSV column name.
    /// This mapper is used in both <see cref="CsvToObjectReader{T}"/> and in <see cref="ObjectToCsvWriter{T}"/>
    /// class.  Instance of this object is expected to passed to constructor of <see cref="CsvToObjectReader{T}"/> and
    /// <see cref="ObjectToCsvWriter{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CsvToObjectMapper<T> : ICsvToObjectMapper<T>
    {
        /// <summary>
        /// Public readonly selectedProperty to store the object to csv mapping. 
        ///  Refer to <see cref="CsvToObjectMap {T}"/> to see the schema of mapping between domain {T} and csv column.
        /// Key of the dictionary will the object selectedProperty name as a string
        /// </summary>
        public Dictionary<string, ICsvToObjectMap<T>> ObjectToCsvMapping { get; private set; } = new Dictionary<string, ICsvToObjectMap<T>>();


        /// <summary>
        /// This is the Mapper method that maps Domain object properties with Csv file column.
        /// Accepts the domain object selectedProperty as expression and the csv column name as string and adds it
        /// to the ObjectToCsvMapping dictionary
        /// </summary>
        /// <typeparam name="TKey">Property name of domain object</typeparam>
        /// <param name="selectedProperty">Domain object selectedProperty info passed as  Expression (e.g. f=> f.FileSizeInBytes)</param>
        /// <param name="csvColumnName">Column name that appears in CSV file</param>
        public void AddMap<TKey>(Expression<Func<T, TKey>> selectedProperty, string csvColumnName)
        {
            if (selectedProperty is null)
                throw new CsvReadWriteException($"selectedProperty cannot be null", ErrorCodes.ParameterNull);
            if (string.IsNullOrEmpty(csvColumnName?.Trim()))
                throw new CsvReadWriteException($"Valid csv column name should be passed into AddMap method parameter", ErrorCodes.ParameterNull);

            if(ObjectToCsvMapping.Any(item => item.Value.CsvColumnName == csvColumnName.Trim()))
                throw new CsvReadWriteException($"Csv column name '{csvColumnName}' already present in the mapping");

            MemberExpression selectedPropertyAsExpresion = ((MemberExpression)selectedProperty.Body);
            var name = selectedPropertyAsExpresion.Member.Name.Trim();
            ObjectToCsvMapping.Remove(name);
            ObjectToCsvMapping.Add(name, new CsvToObjectMap<T>() { CsvColumnName = csvColumnName, PropertyName = name });
        }

    }


}
