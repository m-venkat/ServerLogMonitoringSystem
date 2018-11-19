using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using ServerLogMonitorSystem.FileInfo;
using ServerLogMonitorSystem.Exceptions;

namespace ServerLogMonitorSystem.Parser
{
    
    public class ReadCsvToObject<T> : IReadCsvToObject<T>
    {
        private string _filePath = string.Empty;
        private string _csvContent = string.Empty;
        private readonly CsvReader _reader = null;
        private CsvToObjectClassMap<T> _mapper;
        public ReadCsvToObject(string pathToCsv, CsvToObjectClassMap<T> mapper, bool ignoreDataConversionError = true)
        {
            _mapper = mapper;
            this.IgnoreDataConversionErrors = true;
            ValidateCsvFilePath(pathToCsv);
            _reader = new CsvReader(
                new StringReader(File.ReadAllText(pathToCsv)));
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
        public IReaderConfiguration Configuration
        {
            get { return _reader.Configuration; }
        }

        public IEnumerable<T> Extract()
        {
            _reader.Configuration.RegisterClassMap(_mapper);
            if (this.IgnoreDataConversionErrors) { 
                _reader.Configuration.ReadingExceptionOccurred = ex =>
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine($"Error in Data conversion Record{ex.ReadingContext.Record[0]}\t{ex.ReadingContext.Record[1]}");
                    // Do something with the exception and row data.
                    // You can look at the exception data here too.
                };
            }
            return _reader.GetRecords<T>();
        }

        public IEnumerable<T> ExtractFailedRecords()
        {
            throw new NotImplementedException();
        }
    }
}
