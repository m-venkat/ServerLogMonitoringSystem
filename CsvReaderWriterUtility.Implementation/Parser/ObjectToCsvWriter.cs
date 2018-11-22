using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsvReadWriteUtility.Exceptions;
using CsvReadWriteUtility.Utils;

namespace CsvReadWriteUtility.Parser
{
    
    /// <summary>
    /// Contract for the object that implements functionality writing object to CSV file .
    /// This Interface provides methods to write the object data into CSV file
    /// This Interface is written in generic way to handle any domain object and csv shape.
    /// CSV to Object mapper should be passed as constructor argument to map the CSV fields with Object property
    /// Fields that are part of mapping will be saved in CSV file and rest of the columns will be ignored.
    /// refer <see cref="ICsvToObjectMapper{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectToCsvWriter<T> : IObjectToCsvWriter<T> where T: class
    {
        private readonly IList<List<T>> _groupedObjects;
        private readonly ICsvToObjectMapper<T> _mapper;
        private readonly IReflectionHelper<T> _reflectionHelper;
        private readonly IFileService _fileService;
        private readonly string _targetFolderPath;
        private readonly IList<string> _fileNames;
        private readonly IList<string> _csvFiles = new List<string>();
        public IList<ErrorCodeAndDescription> ErrorsOccured { get; internal set; }

        public bool HasError { get; internal set; }

        public IList<string> PropertyNameToPersist { get; internal set; } =
            new List<string>();
        public IList<string> HeaderColumnNamesInCsvFile { get; internal set; } =
            new List<string>();

        ///  <summary>
        ///  Constructor
        ///  </summary>
        ///  <param name="groupedObjects">List of Lists to serialize to csv File</param>
        ///  <param name="mapper">Object to .csv Mapper</param>
        /// <param name="reflectionHelper">Helper class to reflect the object to retrieve metadata properties</param>
        /// <param name="fileService"></param>
        /// <param name="targetFolderPath">Folder path where the .csv files are expected to persist</param>
        ///  <param name="fileNames">List of .csv file names 
        /// If this parameter is not specified, it will use system timestamp as file name e.g. 21-11-2018 22:00:22.883
        ///  </param>
        public ObjectToCsvWriter(IList<List<T>> groupedObjects, 
                ICsvToObjectMapper<T> mapper, 
                IReflectionHelper<T> reflectionHelper,
                IFileService fileService,
                string targetFolderPath, 
                IList<string> fileNames  )
        {
            _groupedObjects = groupedObjects;
            _mapper = mapper;
            _reflectionHelper = reflectionHelper;
            _fileService = fileService;
            _targetFolderPath = targetFolderPath;
            _fileNames = fileNames;
            MandatoryParameterCheck(groupedObjects, mapper,targetFolderPath,_fileService);
            ExtractHeaderFromMapperFromMapper();
        }


        private void ExtractHeaderFromMapperFromMapper()
        {
            foreach (var header in _mapper.ObjectToCsvMapping)
            {
                PropertyNameToPersist.Add(header.Key);
                HeaderColumnNamesInCsvFile.Add(header.Value.CsvColumnName);
            }
        }

        private bool MandatoryParameterCheck(IList<List<T>> groupedObjects,  ICsvToObjectMapper<T> mapper,string targetFolderPath, IFileService fileService)
        {
            if (groupedObjects == null )
                throw new ArgumentNullException($"Constructor parameter  groupedObjects cannot be blank");
            if (mapper == null)
                throw new ArgumentNullException($"Constructor parameter mapper cannot be blank");
            if(_fileNames.Count != groupedObjects.Count)
                throw new CsvReadWriteException($"Number of file names supplied doesn't match with expected count.  Supplied count:{_fileNames.Count}, Expected count:{_groupedObjects.Count}", ErrorCodes.FileNameListCountNotMatches);
            if (!fileService.DirectoryExists(targetFolderPath))
            {
                try
                {
                    fileService.CreateDirectory(targetFolderPath);//Create if folder doesn't exists
                    fileService.WriteAllText(targetFolderPath+@"\TestFileCreate.txt","sample Content");//Check if file can be created (to check if writable)
                }
                catch (Exception ex)
                {
                    throw new CsvReadWriteException($"Cannot create folder/file in the given path :{targetFolderPath}{Environment.NewLine}{ex.Message}{Environment.NewLine}{ex.StackTrace}", ErrorCodes.CannotWriteFileOrDirectory,ex.StackTrace);
                }
            }
            return true;
        }


        /// <summary>
        /// Writes the object to CSV File
        /// </summary>
        /// <returns></returns>
        public bool Write()
        {
            if (_groupedObjects.Count > 0)
            {
                CreateFileContentForEachList();
                WriteCsvFilesToDisk(_targetFolderPath,_fileNames);
            }
            return false;
        }

        internal void CreateFileContentForEachList()
        {
            foreach (var listObjectForOneFile in _groupedObjects)
            {
                StringBuilder csvDataRows = new StringBuilder(string.Empty);
                foreach (var recordOfT in listObjectForOneFile)
                {
                    csvDataRows.Append(GetCsvStringFromT(recordOfT));
                }
                _csvFiles.Add(csvDataRows.ToString());
            }
        }


        internal void WriteCsvFilesToDisk(string targetFolderPath, IList<string> fileNames)
        {
            for (int fileCtr=0; fileCtr <  _csvFiles.Count;fileCtr++)
            {
                var csvHeaderColumn = string.Empty;
                var fullpath = _fileService.PathCombine(targetFolderPath,
                    fileNames[fileCtr]);
                _fileService.WriteAllText(fullpath, GetCsvColumHeaderFromMapper()+ _csvFiles[fileCtr]);
            }
        }

        private string GetCsvStringFromT(T t)
        {
            StringBuilder csvStringBuilder = new StringBuilder(string.Empty);
            var reflectedList = _reflectionHelper.GetReflectedPropertyInfo(t);
            /*Iterate the column from the Mapper and find the matching columns from the current row and extract only the columns specifed
             in mapper and ignore other columns*/
            bool firstColumn = true;
            foreach (string propName in PropertyNameToPersist)
            {
                IReflectedPropertyInfo propInfo =
                    reflectedList.FirstOrDefault(i => i.PropertyName.Trim().ToUpper() == propName.Trim().ToUpper());
                string columnData = GetCsvColumnDataFromPropertyInfo(propInfo);
                csvStringBuilder.Append(firstColumn ? columnData : "," + columnData);
                firstColumn = false;
            }

            return csvStringBuilder.ToString() + Environment.NewLine;
        }

        private string GetCsvColumHeaderFromMapper()
        {
            string csvHeaderColumn = string.Empty;
            bool firstColumn = true;
            foreach (var header in HeaderColumnNamesInCsvFile)
            {
                csvHeaderColumn += firstColumn == false ? "," : "";
                csvHeaderColumn += "\"" + header + "\"";
                firstColumn = false;
            }
            return csvHeaderColumn += Environment.NewLine;
        }
        private string GetCsvColumnDataFromPropertyInfo(IReflectedPropertyInfo propInfo)
        {
            if ((propInfo.PropertyDataType.ToUpper().Contains("STRING")) ||
                propInfo.PropertyDataType.ToUpper().Contains("DATE"))
            {
                return $"\"" + propInfo.PropertyValue.Replace("\"", "~!@") + "\"".Replace("~!@", "\"");
            }
            else
            {
                return propInfo.PropertyValue.Trim();
            }
            
        }

        /// <summary>
        /// Writes the object to CSV File, Overwrite if the file already Exists
        /// </summary>
        /// <returns></returns>
        public bool Write(bool overwrite)
        {
            Write();
            return false;
        }

    }
}
