using System;
using System.Collections.Generic;
using System.Linq;
using CsvReadWriteUtility.Exceptions;
using CsvReadWriteUtility.Parser;
using CsvReadWriteUtility.Utils;
using ServerLogGrowthTracker.DomainModelGenerator;
using ServerLogGrowthTracker.FileInfo;

namespace TestClient
{
    class Program
    {
        //private static string filePath = $@"C:\Users\muniyandiv\Downloads\TestData\files.csv";
        //private static string fileStat = @"C:\Users\muniyandiv\Downloads\TestData\filestats.csv";

        private static string filePath = $@"C:\Users\venkat\Downloads\TestData\FilesToRead.csv";
        private static string fileStat = @"C:\Users\venkat\Downloads\TestData\FileStatsToRead.csv";

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
            Console.Clear();
            WriteCsvFileFinally(serverLogFileInfoList, serverLogFactInfoList);


            Console.ReadKey();

        }

        static void SlicePrint(IList<List<ServerLogFactGrowthInfo>> sliced)
        {
            foreach (var filegroup in sliced)
            {
                foreach (var f in filegroup)
                {
                    Console.WriteLine(
                        $"{f.FileId}\t{f.FileName}\t{f.TimeStamp}\t{f.SizeInBytes}\t{f.GrowthRateInBytesPerHour}");
                }
                Console.WriteLine("=============================================================================================");
                Console.ReadKey();
            }
        }
        static void WriteCsvFileFinally(List<ServerLogFileInfo> serverLogFileInfoList, List<ServerLogFactInfo> serverLogFactInfoList)
        {
            ServerLogFactGrowthInfoGenerator<ServerLogFactGrowthInfo> slfg = new ServerLogFactGrowthInfoGenerator<ServerLogFactGrowthInfo>(serverLogFileInfoList.Cast<IServerLogFileInfo>().ToList(), serverLogFactInfoList.Cast<IServerLogFactInfo>().ToList());
            var sliced = slfg.GenerateSlicedList();
            SlicePrint(sliced);
            Console.Clear();
            ICsvToObjectMapper<ServerLogFactGrowthInfo> mapper = new CsvToObjectMapper<ServerLogFactGrowthInfo>();
            mapper.AddMap(t => t.FileId, "FileID");
            mapper.AddMap(t => t.FileName, "Name");
            mapper.AddMap(t => t.TimeStampFormatted, "Timestamp");
            mapper.AddMap(t => t.SizeInBytes, "SizeInBytes");
            mapper.AddMap(t => t.GrowthRateInBytesPerHour, "GrowthRateInBytesPerHour");
            ReflectionHelper<ServerLogFactGrowthInfo> rh = new ReflectionHelper<ServerLogFactGrowthInfo>();
            ObjectToCsvWriter<ServerLogFactGrowthInfo> objCsvWriter = new ObjectToCsvWriter<ServerLogFactGrowthInfo>(sliced, mapper, rh, new FileService(), @"C:\Users\venkat\Documents\FinalCsvFiles\");
            //ObjectToCsvWriter<ServerLogFactGrowthInfo> objCsvWriter = new ObjectToCsvWriter<ServerLogFactGrowthInfo>(sliced, mapper, rh, new FileService(), @"C: \Users\muniyandiv\obj to csv\");

            objCsvWriter.Write();
        }

        static List<ServerLogFileInfo> GetServerLogFileInfo()
        {
            CsvToObjectMapper<ServerLogFileInfo> mapper = new CsvToObjectMapper<ServerLogFileInfo>();
            mapper.AddMap((t) => t.FileId, "ID");
            mapper.AddMap(t => t.FileName, "Name");
            CsvToObjectReader<ServerLogFileInfo> readCsv = new CsvToObjectReader<ServerLogFileInfo>(
              filePath, new FileService(), mapper);
            var result = readCsv.Read(out IList<ErrorCodeAndDescription> errorsOccured, out bool parseStatus).ToList();
            foreach (var error in errorsOccured)
            {
                Console.WriteLine($"{error.ErrorCode}-{error.ErrorDescription}");
            }

            return result;
        }

        static List<ServerLogFactInfo> GetServerLogFileFactInfo()
        {
            CsvToObjectMapper<ServerLogFactInfo> mapper = new CsvToObjectMapper<ServerLogFactInfo>();
            mapper.AddMap(t => t.FileId, "FileID");
            mapper.AddMap(t => t.SizeInBytes, "SizeInBytes");
            mapper.AddMap(t => t.TimeStamp, "Timestamp");
            CsvToObjectReader<ServerLogFactInfo> readCsv = new CsvToObjectReader<ServerLogFactInfo>(
                    fileStat, new FileService(), mapper);
            var result = readCsv.Read(out IList<ErrorCodeAndDescription> errorsOccured, out bool parseStatus).ToList();
            foreach (var error in errorsOccured)
            {
                Console.WriteLine($"{error.ErrorCode}-{error.ErrorDescription}");
            }

            return result;
        }


    }
}
