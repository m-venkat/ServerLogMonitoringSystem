using System;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace CsvHelperUser
{
    class Program
    {
        public class FileInfo
        {
            public uint Id { get; set; }
            public String FileName { get; set; }
        }

        public sealed class MyClassMap : ClassMap<FileInfo>
        {
            public MyClassMap()
            {
                Map(m => m.Id).Name("ID");
                Map(m => m.FileName).Name("Name");
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            try
            {
                //var csv = new CsvReader(;
                var csv = new CsvReader(
                    new StringReader(File.ReadAllText(@"C:\Users\venkat\Downloads\TestData\FilesToRead.csv")));
                csv.Configuration.RegisterClassMap<MyClassMap>();
                csv.Configuration.ReadingExceptionOccurred = ex =>
                {
                    //Console.WriteLine(ex.Message);

                    Console.WriteLine($"Error in Data conversion{ex.Data[0]}\t{ex.Data[1]}");
                    // Do something with the exception and row data.
                    // You can look at the exception data here too.
                };
                var records = csv.GetRecords<FileInfo>();
                foreach (var record in records)
                {
                    Console.WriteLine($"{record.Id}\t{record.FileName}");
                }
                Console.WriteLine("CSV Reader construction successfull");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+Environment.NewLine+ex.StackTrace);
                Console.ReadKey();
            }
        }
    }
}
