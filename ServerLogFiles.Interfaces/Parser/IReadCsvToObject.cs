using System.Collections.Generic;
using CsvHelper.Configuration;

namespace ServerLogMonitorSystem.Parser
{
    public interface IReadCsvToObject<out T>
    {
        bool IgnoreDataConversionErrors { get; set; }
        IReaderConfiguration Configuration { get; }
        IEnumerable<T> Extract();
        IEnumerable<T> ExtractFailedRecords();
    }
}