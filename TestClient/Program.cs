using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using ServerLogMonitorSystem.Exceptions;
using ServerLogMonitorSystem.FileInfo;
using ServerLogMonitorSystem.Parser;

namespace TestClient
{
    class Program
    {
        private static string filePath = $@"C:\Users\muniyandiv\Downloads\TestData\files.csv";
        private static string fileStat = @"C:\Users\muniyandiv\Downloads\TestData\filestats.csv";

        //private static string filePath = $@"C:\Users\venkat\Downloads\TestData\FilesToRead.csv";
        //private static string fileStat = @"C:\Users\venkat\Downloads\TestData\FileStatsToRead.csv";
        

        static void Main(string[] args)
        {
            Console.WriteLine("Printing FilesToRead.csv Content");
            Console.WriteLine("======================================================");
            List<ServerLogFileInfo> serverLogFileInfoList = GetServerLogFileInfo();
            List<ServerLogFactInfo> serverLogFactInfoList = GetServerLogFileFactInfo();
            foreach (var record in serverLogFileInfoList)
            {
                Console.WriteLine($"{record.FileId}\t{record.FileName}");
            }

            Console.ReadKey();
            Console.WriteLine("Printing FileStats.csv Content");
            Console.WriteLine("======================================================");
            foreach (var record in serverLogFactInfoList)
            {
                Console.WriteLine($"{record.FileId}\t{record.TimeStamp}\t{record.SizeInBytes}");
            }
            Console.ReadKey();
            //LogFileGrowthDataSetGenerator lg=new LogFileGrowthDataSetGenerator(serverLogFileInfoList,serverLogFactInfoList);
            //List<IServerLogFactGrowthInfo> serverLogFactGrowthInfoList = lg.GenerateLogFileGrowthDataSet().ToList();
            //foreach (var joinedRec in serverLogFactGrowthInfoList)
            //{
            //    Console.WriteLine(
            //        $"{joinedRec.FileId}\t{joinedRec.FileName}\t{joinedRec.TimeStamp}\t{joinedRec.SizeInBytes}");
            //}

            Console.ReadKey();
            
        }

        static List<ServerLogFileInfo> GetServerLogFileInfo()
        {
            CsvToObjectMapper<ServerLogFileInfo> mapper = new CsvToObjectMapper<ServerLogFileInfo>();
            mapper.AddMap((t) => t.FileId, "ID");
            mapper.AddMap(t=> t.FileName,"Name");
            CsvToObjectReader<ServerLogFileInfo> readCsv = new CsvToObjectReader<ServerLogFileInfo>(
              filePath,mapper

          );
            IList<ErrorCodes> listOfErrorCodes;
            IEnumerable<ServerLogFileInfo> logInfoFiles;
            bool result = readCsv.Read(out listOfErrorCodes, out logInfoFiles);
            return logInfoFiles.ToList();
        }

        static List<ServerLogFactInfo> GetServerLogFileFactInfo()
        {
            CsvToObjectMapper<ServerLogFactInfo> mapper = new CsvToObjectMapper<ServerLogFactInfo>();
            mapper.AddMap(t => t.FileId, "FileID");
            mapper.AddMap(t => t.SizeInBytes, "SizeInBytes");
            mapper.AddMap(t => t.TimeStamp, "Timestamp");
            CsvToObjectReader<ServerLogFactInfo> readCsv = new CsvToObjectReader<ServerLogFactInfo>(
                fileStat, mapper

            );
            IList<ErrorCodes> listOfErrorCodes;
            IEnumerable<ServerLogFactInfo> logInfoFiles;
            bool result = readCsv.Read(out listOfErrorCodes, out logInfoFiles);
            return logInfoFiles.ToList();
        }

        //static void oldMethod(string[] args)
        //{
        //    Console.WriteLine("Hello World!");

        //    try
        //    {
        //        //var csv = new CsvReader(;
        //        var csv = new CsvReader(
        //            new StringReader(File.ReadAllText(@"C:\Users\venkat\Downloads\TestData\FilesToRead.csv")));
        //        csv.Configuration.RegisterClassMap<MyClassMap>();
        //        csv.Configuration.ReadingExceptionOccurred = ex =>
        //        {
        //            //Console.WriteLine(ex.Message);

        //            Console.WriteLine($"Error in Data conversion{ex.Data[0]}\t{ex.Data[1]}");
        //            // Do something with the exception and row data.
        //            // You can look at the exception data here too.
        //        };
        //        var records = csv.GetRecords<FileInfo>();
        //        foreach (var record in records)
        //        {
        //            Console.WriteLine($"{record.Id}\t{record.FileName}");
        //        }
        //        Console.WriteLine("CSV Reader construction successfull");
        //        Console.ReadKey();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
        //        Console.ReadKey();
        //    }
        //}
    }
}
