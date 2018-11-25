using System.Collections.Generic;
using CsvReadWriteUtility.Exceptions;
using Microsoft.Extensions.Logging;
using ServerLogGrowthTracker.FileInfo;

namespace ServerLogSizeMonitoring.Console
{
    public class PrintHelper
    {
        public static void PrintErrors(IList<ErrorCodeAndDescription> errors)
        {
            foreach (var error in errors)
            {
                WriteToConsoleAndLog("error", true, true);
            }
        }
        public static void PrintDataConversionErrors(IList<string> dataConversionErrors)
        {
            WriteToConsoleAndLog("Data Conversion Error Exists Check the log for failed records", true, true);
            WriteToConsoleAndLog("DataConversion Failed Records", true, false);
            foreach (var failedRow in dataConversionErrors)
            {
                WriteToConsoleAndLog("{failedRow}", true, false);
            }
        }

        public static void WriteToConsoleAndLog(string message, bool writeToLog, bool writeToConsole)
        {
            if (writeToConsole)
                System.Console.WriteLine(message);
            if (writeToLog)
                Program.Logger.LogInformation(message);

        }


        public static void SlicePrint(IList<List<ServerLogFactGrowthInfo>> sliced)
        {
            foreach (var filegroup in sliced)
            {
                foreach (var f in filegroup)
                {
                    System.Console.WriteLine(
                        $"{f.FileId}\t{f.FileName}\t{f.TimeStamp}\t{f.SizeInBytes}\t{f.MilliSecondsSinceLastLogCreatedForThisFile}\t{f.GrowthRateInBytesPerHour}");
                }
                System.Console.WriteLine("=============================================================================================");
                System.Console.ReadKey();
            }
        }
    }
}