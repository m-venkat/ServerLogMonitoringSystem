using System.Collections.Generic;
using CsvHelper.Configuration;

namespace ServerLogMonitorSystem.Parser
{
    /// <summary>
    /// Contract for the object that implements CSV to Object conversion logic.
    /// This Interface provides methods to extract data from CSV file and populate it with
    /// domain objects.  This Inteface is written in generic way to handle any type and csv shape.
    /// CSV to Object mapper should be passed as constructor argument to map the CSV fields with Object property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadCsvToObject<out T>
    {
        /// <summary>
        /// Flag to indicate whether to throw exception on data conversion errors from CSV to Object or to Ignore
        /// Default value is to Ignore/skip the data conversion errors
        /// </summary>
        bool IgnoreDataConversionErrors { get; set; }
        IReaderConfiguration Configuration { get; }//To be removed

        /// <summary>
        /// Extracts the CSV file data as IEnumerable<T>
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> Extract();
        IEnumerable<string[]> ExtractFailedRecords();
    }
}

