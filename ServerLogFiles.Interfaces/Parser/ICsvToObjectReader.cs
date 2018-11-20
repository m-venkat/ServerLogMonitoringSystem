using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ServerLogMonitorSystem.Exceptions;

namespace ServerLogMonitorSystem.Parser
{
  

    /// <summary>
    /// Contract for the object that implements CSV to Object conversion logic.
    /// This Interface provides methods to extract data from CSV file and populate it with
    /// domain objects.  This Inteface is written in generic way to handle any type and csv shape.
    /// CSV to Object mapper should be passed as constructor argument to map the CSV fields with Object property
    /// </summary>
    /// <typeparam name="out T"></typeparam>
    public interface ICsvToObjectReader<T>
    {
        
        /// <summary>
        /// Extracts and deserializes the CSV file data as IEnumerable<T> out argument,
        /// returns false if it cannot deserialize and returns all the validatino Errors as out parameter
        /// </summary>
        /// <param name="validationErrors">list of Validation Errors as Enum</param>
        /// <param name="domainObjects">returns extracted list of Objects</param>
        /// <returns>true if deserialization is successful else false</returns>
        bool Extract(out IList<ErrorCodes> validationErrors, out IEnumerable<T> domainObjects);


        /// <summary>
        /// Slices the flat dataset into multiple dataset based on the given input property Expression
        /// </summary>
        /// <param name="keyProperty">input property/field to group/slice the dataset</param>
        /// <returns></returns>
        IEnumerable<IEnumerable<T>> SliceDataSetByKey(Expression<Func<T>> keyProperty);

        /// <summary>
        /// Extracts the failed records as IEnumerable<string[]>
        /// </summary>
        /// <returns></returns>
        IEnumerable<string[]> ExtractFailedRecords();
    }
}

