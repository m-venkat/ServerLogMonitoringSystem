using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CsvReadWriteUtility.Exceptions;
using CsvReadWriteUtility.Utils;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("ServerLogMonitoringSystem.Tests")]

namespace CsvReadWriteUtility.Parser
{
    
    public class CsvToObjectReader<T> : ICsvToObjectReader<T>
    {
        #region Private Variables
        private readonly string _pathToCsv;
        private readonly IFileService _fileService;
        private readonly ILogger<CsvToObjectReader<T>> _logger;
        private string[] _csvContentLines = new string[] { };
        private readonly IList<ErrorCodes> _validationErrors = new List<ErrorCodes>();
        private readonly ICsvToObjectMapper<T> _mapper ;
        private readonly bool _ignoreDataConversionError ;
        private readonly bool _ignoreColumnCountMismatch;
        private bool _ignoreEmptyFile;
        readonly Dictionary<string, string> _csvHeaderAndData = new Dictionary<string, string>();
        private string[] _failedParsingRecords = new string[] { };

        #endregion

        #region Public Properties
        public IList<string> HeaderColumnNamesInCsvFile { get; internal set; } =
            new List<string>();

        public IList<ErrorCodeAndDescription> ErrorsOccured { get; internal set; } =
            new List<ErrorCodeAndDescription>();
       
        public bool HasError => ErrorsOccured.Count > 0;

        public IList<string> ExtractFailedRows { get; internal set; } =
            new List<string>();
        #endregion

        #region Constructors

        /// <summary>
        /// Constructs the Csv to Object reader instance.  Most of the parameters are optional parameters
        /// with default value.  Override the default value with the custom values
        /// </summary>
        /// <param name="pathToCsv">Complete file path to the .csv/txt file</param>
        /// <param name="mapper">instance of Csv File to Domain object mapper <see cref="CsvToObjectMapper{T}"/></param>
        /// <param name="fileService">FileService for all file related operations<see cref="IFileService"/></param>
        /// <param name="loggerFactory">Logger factory</param>
        /// <param name="ignoreEmptyFile">should empty file be ignored and not marked as error?[default=true]</param>
        /// <param name="ignoreColumnCountMismatch">Should ignore the additional columns if present and not report error?[default=true]</param>
        /// <param name="ignoreDataConversionError">Should ignore the rows failing because of schema/data conversion issues?[default=true]</param>
        public CsvToObjectReader(
                                string pathToCsv,
                                IFileService fileService,
                                ICsvToObjectMapper<T> mapper,
                                ILoggerFactory loggerFactory,
                                bool ignoreEmptyFile =true,
                                bool ignoreColumnCountMismatch = true,
                                bool ignoreDataConversionError = true
                              )
        {
            /*
             Initialize all the initial configuration settings
             */
            this._pathToCsv = pathToCsv;
            this._fileService = fileService;
            this._mapper = mapper;
            this._ignoreDataConversionError = ignoreDataConversionError;
            this._ignoreEmptyFile = ignoreEmptyFile;
            this._ignoreColumnCountMismatch = ignoreColumnCountMismatch;
            this._logger = loggerFactory.CreateLogger<CsvToObjectReader<T>>();

        }

        /// <summary>
        /// Constructs the Csv to Object reader instance.  Most of the parameters are optional parameters
        /// with default value.  Override the default value with the custom values
        /// </summary>
        /// <param name="mapper">instance of Csv File to Domain object mapper <see cref="CsvToObjectMapper{T}"/></param>
        /// <param name="loggerFactory"></param>
        /// <param name="fileContent">Content of .csv/txt file</param>
        /// <param name="ignoreEmptyFile">should empty file be ignored and not marked as error?[default=true]</param>
        /// <param name="ignoreColumnCountMismatch">Should ignore the additional columns if present and not report error?[default=true]</param>
        /// <param name="ignoreDataConversionError">Should ignore the rows failing because of schema/data conversion issues?[default=true]</param>
        public CsvToObjectReader(
            CsvToObjectMapper<T> mapper,
            ILoggerFactory loggerFactory,
            string fileContent,
            bool ignoreEmptyFile = true,
            bool ignoreColumnCountMismatch = true,
            bool ignoreDataConversionError = true
        )
        {
            /*
             Initialize all the initial configuration settings
             */
            this._csvContentLines= fileContent?.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
            );
            this._mapper = mapper;
            this._ignoreDataConversionError = ignoreDataConversionError;
            this._ignoreEmptyFile = ignoreEmptyFile;
            this._ignoreColumnCountMismatch = ignoreColumnCountMismatch;
            this._logger = loggerFactory.CreateLogger<CsvToObjectReader<T>>();

        }

        /// <summary>
        /// Constructs the Csv to Object reader instance.  Most of the parameters are optional parameters
        /// with default value.  Override the default value with the custom values
        /// </summary>
        /// <param name="fileContentLines">Content of .csv/txt file in a string array</param>
        /// <param name="mapper">instance of Csv File to Domain object mapper <see cref="CsvToObjectMapper{T}"/></param>
        /// <param name="loggerFactory"></param>
        /// <param name="headerPresentInFirstRow">Does this csv/text file has header row in first line?[default=true]</param>
        /// <param name="mustMatchExpectedHeader">Should this csv file headers match with the header provided in mapper?[default=true]</param>
        /// <param name="ignoreEmptyFile">should empty file be ignored and not marked as error?[default=true]</param>
        /// <param name="ignoreColumnCountMismatch">Should ignore the additional columns if present and not report error?[default=true]</param>
        /// <param name="ignoreDataConversionError">Should ignore the rows failing because of schema/data conversion issues?[default=true]</param>
        public CsvToObjectReader(
            string[] fileContentLines,
            CsvToObjectMapper<T> mapper,
            ILoggerFactory loggerFactory,
            bool headerPresentInFirstRow = true,
            bool mustMatchExpectedHeader = true,
            bool ignoreEmptyFile = true,
            bool ignoreColumnCountMismatch = true,
            bool ignoreDataConversionError = true
        )
        {
            /*
             Initialize all the initial configuration settings
             */
            this._csvContentLines = fileContentLines;
            this._mapper = mapper;
            this._ignoreDataConversionError = ignoreDataConversionError;
            this._ignoreEmptyFile = ignoreEmptyFile;
            this._ignoreColumnCountMismatch = ignoreColumnCountMismatch;
            this._logger = loggerFactory.CreateLogger<CsvToObjectReader<T>>();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Internal Helper method to add ErrorsOccured to public Error
        /// </summary>
        /// <param name="codes"></param>
        /// <param name="description"></param>
        private void AddError(ErrorCodes codes, string description )
        {
            ErrorsOccured.Add(new ErrorCodeAndDescription() {ErrorCode = codes, ErrorDescription = description});
            _logger.LogError($"{codes}-{description}");
        }



        /// <summary>
        /// Runs series of pre validations on the given CSV file before starting extracting data
        /// </summary>
        internal bool PreExtractValidation()
        {
            bool validationFailed = false;

            if (_pathToCsv == null && _csvContentLines.Length==0) {
                AddError(ErrorCodes.NullPath,"Please supply the file content or provide the path to the file in constructor argument");
                validationFailed = true;
            }
            if (!string.IsNullOrEmpty(_pathToCsv) && _fileService == null)
            {
                AddError(ErrorCodes.ParameterNull, $"Constructor parameter 'fileService' cannot be null");
                validationFailed = true;
            }

            if (_mapper == null)
            {
                AddError(ErrorCodes.ParameterNull, $"Constructor parameter '_mapper' cannot be null");
                validationFailed = true;
            }
            if (!string.IsNullOrEmpty(_pathToCsv) && _fileService != null && !_fileService.FileExists(_pathToCsv)) { 
                AddError(ErrorCodes.PathNotExists,$"{_pathToCsv}  does not exists");
                validationFailed = true;
            }
            if (!string.IsNullOrEmpty(_pathToCsv) && _fileService != null && _fileService.PathGetExtension(_pathToCsv).ToUpper() != ".CSV")
            { 
                AddError(ErrorCodes.InvalidFileExtension,$"Invalid file extension {_fileService.PathGetExtension(_pathToCsv)}. Only .csv is supported ");
                validationFailed = true;
            }
            if (!validationFailed && _fileService != null && _mapper != null) { 
                try
                {
                    if(_csvContentLines.Length == 0)
                        _csvContentLines = _fileService.ReadAllLines(_pathToCsv);
                }
                catch (Exception ex)
                {
                    AddError(ErrorCodes.CannotReadFile,$"{_pathToCsv} Cannot be read\n Exception:{ex.Message}\nStacktrace:{ex.StackTrace}");
                    return false;
                }
                if( 
                    (_csvContentLines.Length == 0 ||
                      (_csvContentLines.Length == 1 && string.IsNullOrEmpty(_csvContentLines[0].Trim()))))
                {
                    AddError(ErrorCodes.FileEmpty, "Csv file Content is empty");
                    if (!_ignoreEmptyFile)
                    {
                        throw new CsvReadWriteException(ErrorCodes.FileEmpty);
                    }
                    return false;
                }
                this.HeaderColumnNamesInCsvFile = SplitLineOfTextIntoCsv.Split(_csvContentLines[0]);
                return IsCsvColumnNamesAsExpectedWithoutDuplicateColumnNames();//Throws Exception if csv columns are not as expected
            }

            return false;
        }

        
        /// <summary>
        /// Takes a line of text and converts into domain object T
        /// </summary>
        /// <param name="lineOfRecordFromCsv">line of text from csv file</param>
        /// <param name="convertedObj">Converted domain object</param>
        /// <returns></returns>
        internal bool ConvertCsvRecordToObject(string lineOfRecordFromCsv, out T convertedObj)
        {
            _csvHeaderAndData.Clear();
            string[] columnData = SplitLineOfTextIntoCsv.Split(lineOfRecordFromCsv);
            for (ushort i=0;i<=HeaderColumnNamesInCsvFile.Count;i++)
            {
                if(i < HeaderColumnNamesInCsvFile.Count && i < columnData.Length )
                    _csvHeaderAndData.Add(HeaderColumnNamesInCsvFile[i], columnData[i]);
            }

            if (_ignoreColumnCountMismatch == false && columnData.Length != HeaderColumnNamesInCsvFile.Count)
            {
                AddError(ErrorCodes.ColumnCountMismatch,$"Number of columns present in CSV doesn't matches with expected mapper. Count of columns present in CSV={HeaderColumnNamesInCsvFile.Count}, No of columns in mapper:{columnData.Length}  ");
                convertedObj = default(T);
                return false;
            }

            T domainObj = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] properties = domainObj.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                //Get Each property name from list of properties
                if (this._mapper.ObjectToCsvMapping.ContainsKey(property.Name)) //Ignore the object properties if no mapping proeprties passed
                {
                    ICsvToObjectMap<T> map = this._mapper.ObjectToCsvMapping[property.Name];
                    string csvColumnNameFromMap = map.CsvColumnName;
                    string columnValue = _csvHeaderAndData[csvColumnNameFromMap];
                    try
                    {
                        property.SetValue(domainObj, Convert.ChangeType(columnValue, property.PropertyType), null);
                    }
                    catch (Exception ex
                    ) //Handle data conversion failure(if csv data is not compatible with object datatype)
                    {

                        _logger.LogError($"{ex.Message}\n{ex.StackTrace}");
                        convertedObj = default(T);
                        this.ExtractFailedRows.Add(lineOfRecordFromCsv);
                        var error =
                            $"Cannot Convert data '{columnValue}' for Column name {csvColumnNameFromMap}, column data type {property.PropertyType.Name} in file {_pathToCsv}";
                        AddError(ErrorCodes.DataConversionError, error
                            );
                        if(!this._ignoreDataConversionError)
                            throw new CsvReadWriteException(error,ErrorCodes.DataConversionError);
                        return false;
                    }
                }
            }

            convertedObj = domainObj;
            return true;
        }

        
        /// <summary>
        /// Helper method to validate schema and duplicate column names
        /// </summary>
        /// <returns></returns>
        internal bool IsCsvColumnNamesAsExpectedWithoutDuplicateColumnNames()
        {
            bool columnsNotMatches = true;
            foreach (var columnName in HeaderColumnNamesInCsvFile)
            {
                var obj = _mapper.ObjectToCsvMapping.Where(item =>
                        item.Value.CsvColumnName.Trim().ToUpper() == columnName.Trim().ToUpper())
                    .Select(e => (KeyValuePair<string, ICsvToObjectMap<T>>?)e)
                    .FirstOrDefault();
                if (obj == null)
                {
                    this.AddError(ErrorCodes.CsvColumnNameNotFound, $"column {columnName} not found in .csv file");
                    columnsNotMatches = false;
                }
            }
            if (columnsNotMatches == false)
            {
                return false;
            }

            var duplicateColumns = HeaderColumnNamesInCsvFile.Select(i => i.Trim().ToUpper()).GroupBy(word => word)
                .Where(x => x.Count() > 1).ToList(); ;
            foreach (var duplicateColumn in duplicateColumns)
            {
                this.AddError(ErrorCodes.DuplicateColumnNames, $"Column {duplicateColumn} duplicated");
            }
            if (duplicateColumns.Any())
            {
                return false;
            }

            return true;

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Read method returns the extracted IList<typeparamref name="T"/> from .csv file
        /// </summary>
        /// <param name="errorsOccured">List of errors occured</param>
        /// <param name="readSuccessfully">parse status</param>
        /// <returns>IList<typeparamref name="T"/></returns>
        public IList<T> Read(out IList<ErrorCodeAndDescription> errorsOccured, out bool readSuccessfully)
        {
            //Clean up the errors occured and other collections in case if User recursively calls Read method repeatedly
            this.ErrorsOccured.Clear();
            this.HeaderColumnNamesInCsvFile.Clear();
            this.ExtractFailedRows.Clear();
            
            errorsOccured = this.ErrorsOccured;
          
            List<T> convertedObjects = new List<T>();
            if (PreExtractValidation())//If PreExtract Validation is successful, parse the file
            {
                foreach (var line in _csvContentLines.Skip(1))//Assume Always has header
                {
                    if(ConvertCsvRecordToObject(line, out var convertedObj))
                        convertedObjects.Add(convertedObj);
                  
                }
            }
            else
            {
                readSuccessfully= false;
            }
            readSuccessfully = true;
            return convertedObjects;
           
        }

        /// <summary>
        /// Read method returns the extracted IList<typeparamref name="T"/> from .csv file
        /// </summary>
        /// <returns></returns>
        public IList<T> Read()
        {
            return Read(out IList<ErrorCodeAndDescription> _, out _);
        }


        #endregion


    }
}
