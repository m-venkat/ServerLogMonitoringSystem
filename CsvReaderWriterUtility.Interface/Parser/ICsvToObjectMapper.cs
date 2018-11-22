using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using CsvReadWriteUtility.Exceptions;


namespace CsvReadWriteUtility.Parser
{
    /// <summary>
    /// Class responsible for adding Mapping between object selectedProperty and CSV column name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICsvToObjectMapper<T>
    {
        Dictionary<string, ICsvToObjectMap<T>> ObjectToCsvMapping { get; }
        
        
        /// <summary>
        /// This is the Mapper method that maps Domain object properties with Csv file column.
        /// Instance of this class needs to be passed in to the constructor of <see cref="ICsvToObjectReader{T}"/>
        /// </summary>
        /// <typeparam name="TKey">Property name of domain object</typeparam>
        /// <param name="selectedProperty">Domain object selectedProperty</param>
        /// <param name="csvColumnName">Column Name that appears in CSV file</param>
        void AddMap<TKey>(Expression<Func<T, TKey>> selectedProperty, string csvColumnName);


    }


}
