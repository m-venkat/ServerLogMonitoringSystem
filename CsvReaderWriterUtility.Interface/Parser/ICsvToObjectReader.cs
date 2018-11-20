using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    public interface ICsvToObjectReader<T>
    {
         IList<string> HeaderColumnNamesInCsvFile { get;  }

         IList<ErrorCodeAndDescription> ErrorsOccured { get;  }

         bool HasError { get; }

         IList<string> ExtractFailedRows { get;  }

        /// <summary>
        /// Extracts and deserializes the CSV file data as IList<T> 
        /// </summary>
        /// <param name="errorsOccured">List of Errors Occured</param>
        /// <param name="readSuccessfully">Indicates the success/failure of the Read operation</param>
        /// <returns></returns>
        IList<T>  Read(out IList<ErrorCodeAndDescription> errorsOccured, out bool readSuccessfully );

        /// <summary>
        /// Extracts and deserializes the CSV file data as IList<T>. Overloaded method to simple signature to just return
        /// the extracted domain object.  In case of validation/extraction failure "null" will be returned.
        /// Errors can be examined thru ErrorsOccured property
        /// </summary>
        /// <returns></returns>
        IList<T> Read();


    }
}

