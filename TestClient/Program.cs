using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using CsvReadWriteUtility.Exceptions;
using CsvReadWriteUtility.Parser;
using CsvReadWriteUtility.Utils;
using Microsoft.Extensions.Logging;
using ServerLogGrowthTracker.DomainModelGenerator;
using ServerLogGrowthTracker.FileInfo;
using Microsoft.Extensions.DependencyInjection;

namespace ServerLogSizeMonitoring.Console
{
    class Program
    {
        public static LoggerFactory LoggerFactory = new LoggerFactory();
        public static InputParams Parameters = new InputParams();
        public static ILogger Logger;
    

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                InputParamsHelper.DisplayHelp(); //Exit after displaying help prompt;
            }
            Parameters = InputParamsHelper.ParseCommandLineParams(args);
          
            LoggerFactory.AddFile("Logs/ServerLogMonitoringLog-{Date}.txt");
            Logger = LoggerFactory.CreateLogger<Program>();
            List<ServerLogFileInfo> serverLogFileInfoList = GetServerLogFileInfo();//Get the files.csv as cv collection
            List<ServerLogFactInfo> serverLogFactInfoList = GetServerLogFileFactInfo();// Get the file status.cs as collection
            WriteCsvFileFinally(serverLogFileInfoList, serverLogFactInfoList);//Write the final sliced data sets as csv to file

            System.Console.WriteLine("\n\nRefer to the log folder for more details. ");
            System.Console.WriteLine("\n\nPress any key to exit");
            System.Console.ReadKey();
            Environment.Exit(0);
        }

   
        static void WriteCsvFileFinally(List<ServerLogFileInfo> serverLogFileInfoList, List<ServerLogFactInfo> serverLogFactInfoList)
        {
            try
            {
                ServerLogFactGrowthInfoGenerator<ServerLogFactGrowthInfo> slfg =
                    new ServerLogFactGrowthInfoGenerator<ServerLogFactGrowthInfo>(
                        serverLogFileInfoList.Cast<IServerLogFileInfo>().ToList(),
                        serverLogFactInfoList.Cast<IServerLogFactInfo>().ToList());

                var sliced = slfg.GenerateSlicedList();
                PrintHelper.WriteToConsoleAndLog($"Calculation Completed for file growth rate, got back {sliced.Count()} data sets", true, true);
                PrintHelper.WriteToConsoleAndLog($"Attempting writing to csv files at {Parameters.OutputFolder}", true, true);
                ICsvToObjectMapper<ServerLogFactGrowthInfo> mapper = new CsvToObjectMapper<ServerLogFactGrowthInfo>();
                mapper.AddMap(t => t.FileId, "FileID");
                mapper.AddMap(t => t.FileName, "Name");
                mapper.AddMap(t => t.TimeStampFormatted, "Timestamp");
                mapper.AddMap(t => t.SizeInBytes, "SizeInBytes");
                mapper.AddMap(t => t.GrowthRateInBytesPerHour, "GrowthRateInBytesPerHour");
                ReflectionHelper<ServerLogFactGrowthInfo> rh = new ReflectionHelper<ServerLogFactGrowthInfo>();
                List<string> fileNames = sliced.Select(list => list?.FirstOrDefault()?.FileId + ".csv").ToList();
                ObjectToCsvWriter<ServerLogFactGrowthInfo> objCsvWriter =
                    new ObjectToCsvWriter<ServerLogFactGrowthInfo>(sliced, mapper, new LoggerFactory(), rh,
                        new FileService(), Parameters.OutputFolder, fileNames);
                objCsvWriter.Write();
                PrintHelper.WriteToConsoleAndLog($"Completed writing to {Parameters.OutputFolder}. No of files created {sliced.Count()}", true, true);
            }
            catch (CsvReadWriteException csvException)
            {
                PrintHelper.WriteToConsoleAndLog($"{csvException.ErrorCode}\n{csvException.Message}\n{csvException.StackTrace}", true, false);
                PrintHelper.WriteToConsoleAndLog($"{csvException.ErrorCode}\n{csvException.Message}\nCheck Log for more details", false, true);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                PrintHelper.WriteToConsoleAndLog($"{ex.Message}\n{ex.StackTrace}", true, false);
                PrintHelper.WriteToConsoleAndLog($"{ex.Message}\nCheck Log for more details", true, true);
                Environment.Exit(0);
            }
        }

        static List<ServerLogFileInfo> GetServerLogFileInfo()
        {
            CsvToObjectMapper<ServerLogFileInfo> mapper = new CsvToObjectMapper<ServerLogFileInfo> ();
          

            mapper.AddMap((t) => t.FileId, "ID");
            mapper.AddMap(t => t.FileName, "Name");
            List<ServerLogFileInfo> result = new List<ServerLogFileInfo>();
            try
            {
                var readCsv = new CsvToObjectReader<ServerLogFileInfo>(
                    Parameters.FilePath, new FileService(), mapper, LoggerFactory);
                PrintHelper.WriteToConsoleAndLog("CsvToObjectReader instance constructed", true, false);
                var res = readCsv.Read(out IList<ErrorCodeAndDescription> errorsOccured, out bool parseStatus);
                ValidateAndLogExtractedList("ServerLogFileInfo", Parameters.FilePath, parseStatus, readCsv.ErrorsOccured, readCsv.ExtractFailedRows, res?.Count());
                return res?.ToList();

            }
            catch (CsvReadWriteException csvException)
            {
                PrintHelper.WriteToConsoleAndLog($"{csvException.ErrorCode}\n{csvException.Message}\n{csvException.StackTrace}", true, false);
                PrintHelper.WriteToConsoleAndLog($"{csvException.ErrorCode}\n{csvException.Message}\nCheck Log for more details", false, true);
            }
            catch (Exception ex)
            {
                PrintHelper.WriteToConsoleAndLog($"{ex.Message}\n{ex.StackTrace}", true, false);
                PrintHelper.WriteToConsoleAndLog($"{ex.Message}\nCheck Log for more details", true, true);
            }

            return null;
        }

       
       
        static List<ServerLogFactInfo> GetServerLogFileFactInfo()
        {
            CsvToObjectMapper<ServerLogFactInfo> mapper = new CsvToObjectMapper<ServerLogFactInfo>();
            mapper.AddMap(t => t.FileId, "FileID");
            mapper.AddMap(t => t.SizeInBytes, "SizeInBytes");
            mapper.AddMap(t => t.TimeStamp, "Timestamp");

            List<ServerLogFactInfo> result = new List<ServerLogFactInfo>();
            CsvToObjectReader<ServerLogFactInfo> readCsv;
            try
            {
                readCsv = new CsvToObjectReader<ServerLogFactInfo>(
                    Parameters.FactFilePath, new FileService(), mapper, LoggerFactory);
                PrintHelper.WriteToConsoleAndLog("CsvToObjectReader instance constructed", true, false);
                var res = readCsv.Read(out IList<ErrorCodeAndDescription> errorsOccured, out bool parseStatus);
                ValidateAndLogExtractedList("ServerLogFactInfo",Parameters.FactFilePath,parseStatus, readCsv.ErrorsOccured, readCsv.ExtractFailedRows,res?.Count());
                return res?.ToList();
                
            }
            catch (CsvReadWriteException csvException)
            {
                PrintHelper.WriteToConsoleAndLog($"{csvException.ErrorCode}\n{csvException.Message}\n{csvException.StackTrace}", true, false);
                PrintHelper.WriteToConsoleAndLog($"{csvException.ErrorCode}\n{csvException.Message}\nCheck Log for more details", false, true);
            }
            catch (Exception ex)
            {
                PrintHelper.WriteToConsoleAndLog($"{ex.Message}\n{ex.StackTrace}", true, false);
                PrintHelper.WriteToConsoleAndLog($"{ex.Message}\nCheck Log for more details", true, true);
                Environment.Exit(0);
            }

            return null;

        }

        private static void ValidateAndLogExtractedList(string objType, string filePath, bool parseStatus, IList<ErrorCodeAndDescription> errorsOccured, IList<string> failedRows, int? numberOfRecords)
        {
           
            if (parseStatus == false)
            {
                PrintHelper.WriteToConsoleAndLog($"CsvToObjectReader to List Extraction failed for {Parameters.FactFilePath}", true, true);
                PrintHelper.PrintErrors(errorsOccured);
            }
            else
            {
                PrintHelper.WriteToConsoleAndLog($"{filePath} has been extracted to List<{objType}> collection. No of Records: {numberOfRecords}", true, true);
                if (failedRows.Count == 0)
                {
                    PrintHelper.WriteToConsoleAndLog("No Data Conversion error all the records were extracted to Collection", true, true);
                   
                }
                else
                {
                    PrintHelper.PrintDataConversionErrors(failedRows);
                }
            }
        }
    }
}
