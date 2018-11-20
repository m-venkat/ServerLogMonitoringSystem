using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using CsvReadWriteUtility.Exceptions;


namespace CsvReadWriteUtility.Parser
{

    public interface ICsvToObjectMap<T>
    {
         MemberExpression Property { get;  set; }
         String CsvColumnName { get;  set; }
    }

    /// <summary>
    /// Class responsible for adding Mapping between object property and CSV column name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public  interface ICsvToObjectMapper<T>
    {
         Dictionary<string, ICsvToObjectMap<T>> ObjectToCsvMapping { get;  set; }
        /// <summary>
        /// This is the Mapper method that maps Domain object properties with Csv file column.
        /// Instance of this class needs to be passed in to the constructor of <see cref="ICsvToObjectReader{T}"/>
        /// </summary>
        /// <typeparam name="TKey">Property name of domain object</typeparam>
        /// <param name="property">Domain object property</param>
        /// <param name="csvColumnName">Column Name that appears in CSV file</param>
        void AddMap<TKey>(Expression<Func<T, TKey>> property, string csvColumnName);


    }


}
