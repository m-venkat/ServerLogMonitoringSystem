using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using LogFileGrowthTracker.FileInfo;
using LogFileGrowthTracker.Parser;

namespace TestClient
{
    class Program
    {
        private static string filePath = $@"C:\Users\muniyandiv\Downloads\TestData\files.csv";
        private static string fileStat = @"C:\Users\muniyandiv\Downloads\TestData\filestats.csv";

        //private static string filePath = $@"C:\Users\venkat\Downloads\TestData\FilesToRead.csv";
        //  private static string fileStat = @"C:\Users\venkat\Downloads\TestData\FileStatsToRead.csv";
        public class FileInfo
        {
            public uint Id { get; set; }
            public String FileName { get; set; }
        }

        public sealed class ServerLogFileInfoCsvMap : CsvToObjectClassMap<ServerLogFileInfo>
        {
            public ServerLogFileInfoCsvMap()
            {
                Map(m => m.FileId).Name("ID");
                Map(m => m.FileName).Name("Name");
            }
        }

        public sealed class ServerLogFactInfoCsvMap : CsvToObjectClassMap<ServerLogFactInfo>
        {
            public ServerLogFactInfoCsvMap()
            {
                Map(m => m.FileId).Name("FileID");
                Map(m => m.TimeStamp).Name("Timestamp");
                Map(m => m.SizeInBytes).Name("SizeInBytes");
            }
        }

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
            LogFileGrowthDataSetGenerator lg=new LogFileGrowthDataSetGenerator(serverLogFileInfoList,serverLogFactInfoList);
            List<IServerLogFactGrowthInfo> serverLogFactGrowthInfoList = lg.GenerateLogFileGrowthDataSet().ToList();
            foreach (var joinedRec in serverLogFactGrowthInfoList)
            {
                Console.WriteLine(
                    $"{joinedRec.FileId}\t{joinedRec.FileName}\t{joinedRec.TimeStamp}\t{joinedRec.SizeInBytes}");
            }

            Console.ReadKey();



        }

        static List<ServerLogFileInfo> GetServerLogFileInfo()
        {
            ReadCsvToObject<ServerLogFileInfo> extractObj = new ReadCsvToObject<ServerLogFileInfo>
            (
                filePath,
                new ServerLogFileInfoCsvMap()
            );
            extractObj.IgnoreDataConversionErrors = true;
            var records = extractObj.Extract();
            return records.ToList();
            
        }

        static List<ServerLogFactInfo> GetServerLogFileFactInfo()
        {
            ReadCsvToObject<ServerLogFactInfo> extractObj = new ReadCsvToObject<ServerLogFactInfo>
            (
                fileStat,
                new ServerLogFactInfoCsvMap()
            );
            var records = extractObj.Extract();
            return records.ToList();

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
