using System;
using System.Collections.Generic;
using System.Text;
using CsvReadWriteUtility.Exceptions;

namespace CsvReadWriteUtility.Parser
{
    
    /// <summary>
    /// Contract for the object that implements CSV to Object conversion logic.
    /// This Interface provides methods to extract data from CSV file and populate it with
    /// domain objects.  This Interface is written in generic way to handle any domain object and csv shape.
    /// CSV to Object mapper should be passed as constructor argument to map the CSV fields with Object property
    /// refer <see cref="ICsvToObjectMapper{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectToCsvWriter<T>
    {
       
        IList<ErrorCodeAndDescription> ErrorsOccured { get; }

        bool HasError { get; }

      
        
        /// <summary>
        /// Writes the object to CSV File, Overwrite if the file already Exists
        /// </summary>
        /// <returns></returns>
        bool Write(bool overwrite);

    }
}
