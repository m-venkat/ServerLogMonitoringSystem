using System.Text;
using Microsoft.Extensions.Logging;

namespace ServerLogSizeMonitoring.Console
{
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