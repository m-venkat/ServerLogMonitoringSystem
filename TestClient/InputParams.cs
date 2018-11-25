using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServerLogGrowthTracker.FileInfo;
using CsvReadWriteUtility.Exceptions;
using CsvReadWriteUtility.Parser;
using CsvReadWriteUtility.Utils;

namespace ServerLogSizeMonitoring.Console
{

    public class DiProvider
    {
        /// <summary>
        /// Gets the service provider based on the input arguments.
        /// </summary>
        /// <param name="programInput"></param>
        /// <returns></returns>
        public static ServiceProvider GetServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton<IServerLogFileInfo, ServerLogFileInfo>()
                .AddSingleton<IServerLogFactInfo, ServerLogFactInfo>()
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .AddSingleton(typeof(ICsvToObjectMapper<>), typeof(CsvToObjectMapper<>))
                .AddSingleton(typeof(IObjectToCsvWriter<>), typeof(ObjectToCsvWriter<>))
                .AddSingleton(typeof(IServerLogFactGrowthInfo), typeof(ServerLogFactGrowthInfo))
                .BuildServiceProvider();

        }
    }
    public class InputParams
    {
        #region Public Properties

        public string FilePath { get; set; }
        public string FactFilePath { get; set; }
        public string OutputFolder { get; set; }
        public LogLevel LogLevel { get; set; }
    }
}
#endregion