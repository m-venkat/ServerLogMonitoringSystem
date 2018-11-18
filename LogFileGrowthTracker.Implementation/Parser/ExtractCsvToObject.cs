using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using LogFileGrowthTracker.Exceptions;
using LogFileGrowthTracker.FileInfo;

namespace LogFileGrowthTracker.Parser
{
 
    public sealed class MyClassMap : CsvToObjectMapper<IServerLogFileInfo>
    {
        public MyClassMap()
        {
            Map(m => m.FileId).Name("ID");
            Map(m => m.FileName).Name("Name");
        }
    }
    //public class CsvToObjectMap<T> : ICsvToObjectMap<T>
    //{
    //     public Expression<Func<T>> PropertyExpression { get; set; }
    //     public string ColumnNameInCsv { get; set; }
    //     public string PropertyColumnName { get; set; }
    //}


    //public class CsvToObjectMapper<T> : ICsvToObjectMapper<T>
    //{
    //    public CsvToObjectMapper()
    //    {
    //        CsvToObjectMap = new Dictionary<string, ICsvToObjectMap<T>>();
    //    }
    //    public void AddMapping(Expression<Func<T>> objProperty, string columnNameInCsvFile)
    //    {
    //        if(string.IsNullOrEmpty(columnNameInCsvFile)) throw new LogFileGrowthTrackerException("Csv column name is missing",ErrorCodes.CsvColumnNameMissing,"");
    //        var name = ((MemberExpression)objProperty.Body).Member.Name;
    //        CsvToObjectMap.Remove(name);//Remove if it already exists
    //        CsvToObjectMap.Add(
    //                name,
    //                new CsvToObjectMap<T>() //Get from a factory VM: To be Optimized
    //                {
    //                    ColumnNameInCsv = name,
    //                    PropertyExpression = objProperty,
    //                    PropertyColumnName = columnNameInCsvFile
    //                }
    //            );
    //    }

    //    public Dictionary<string, ICsvToObjectMap<T>> CsvToObjectMap {get; private set; }
    
    //};
    

    class ExtractCsvToObject<T> : IExtractCsvToObject<T>
    {
        private string _filePath = string.Empty;
        private string _csvContent = string.Empty;
        private CsvToObjectMapper<T> _mapper = null;
        public ExtractCsvToObject(string pathToCsv, CsvToObjectMapper<T> mapper)
        {
            this._mapper = mapper;
            ValidateCsvFilePath(pathToCsv);
        }

        private void ValidateCsvFilePath(string pathToCsv)
        {
            if (pathToCsv == null)
                throw new LogFileGrowthTrackerException("Path to CSV File is expected", ErrorCodes.NullPath, string.Empty);
            else if (!File.Exists(pathToCsv))
                throw new LogFileGrowthTrackerException($"Given file {pathToCsv} not found ",ErrorCodes.PathNotExists,string.Empty);
            else if (Path.GetExtension(pathToCsv).ToUpper() != ".CSV")
                throw new LogFileGrowthTrackerException($"Invalid file extension (Path.GetExtension(pathToCsv)), Expected .csv extension",ErrorCodes.InvalidFileExtension,"");
            try
            {
                _csvContent = File.ReadAllText(pathToCsv);
            }
            catch (Exception ex)
            {
                throw new LogFileGrowthTrackerException($"Unable to read file", ErrorCodes.CannotReadFile, ex.StackTrace);
            }

            if (string.IsNullOrEmpty(_csvContent.Trim()))
            {
                throw new LogFileGrowthTrackerException($"File is Empty", ErrorCodes.FileEmpty, "");
            }
        }

        
        public bool IgnoreDataConversionErrors { get; set; }
        public IEnumerable<T> Extract<CsvToObjectMapper<T1>>()
        {
            var csv = new CsvReader(
                new StringReader(File.ReadAllText(@"C:\Users\venkat\Downloads\TestData\FilesToRead.csv")));
            csv.Configuration.RegisterClassMap<CsvToObjectMapper<T1>>();
        }

        public IEnumerable<T> ExtractFailedRecords()
        {
            throw new NotImplementedException();
        }
    }
}
