using System.Collections.Generic;
using CsvHelper.Configuration;

namespace LogFileGrowthTracker.Parser
{
    public interface IReadCsvToObject<T>
    {
        bool IgnoreDataConversionErrors { get; set; }
        IReaderConfiguration Configuration { get; }
        IEnumerable<T> Extract();
        IEnumerable<T> ExtractFailedRecords();
    }
}