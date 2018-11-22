using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CsvReadWriteUtility.Exceptions;
using CsvReadWriteUtility.Utils;


namespace CsvReadWriteUtility.Parser
{

    public class CsvToObjectReader<T> : ICsvToObjectReader<T>
    {
        #region Private Variables
        private readonly string _pathToCsv;
        private readonly IFileService _fileService;
        private string[] _csvContentLines = new string[] { };
        private readonly IList<ErrorCodes> _validationErrors = new List<ErrorCodes>();
        private readonly ICsvToObjectMapper<T> _mapper ;
        private readonly bool _ignoreDataConversionError ;
        private readonly bool _headerPresentInFirstRow;
        private readonly bool _mustMatchExpectedHeader;
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
        /// <param name="headerPresentInFirstRow">Does this csv/text file has header row in first line?[default=true]</param>
        /// <param name="mustMatchExpectedHeader">Should this csv file headers match with the header provided in mapper?[default=true]</param>
        /// <param name="ignoreEmptyFile">should empty file be ignored and not marked as error?[default=true]</param>
        /// <param name="ignoreColumnCountMismatch">Should ignore the additional columns if present and not report error?[default=true]</param>
        /// <param name="ignoreDataConversionError">Should ignore the rows failing because of schema/data conversion issues?[default=true]</param>
        public CsvToObjectReader(
                                string pathToCsv,
                                IFileService fileService,
                                ICsvToObjectMapper<T> mapper,
                                bool headerPresentInFirstRow = true,
                                bool mustMatchExpectedHeader = true,
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
            this._mustMatchExpectedHeader = mustMatchExpectedHeader;
            this._mapper = mapper;
            this._ignoreDataConversionError = ignoreDataConversionError;
            this._headerPresentInFirstRow = headerPresentInFirstRow;
            this._ignoreEmptyFile = ignoreEmptyFile;
            this._ignoreColumnCountMismatch = ignoreColumnCountMismatch;
            _mustMatchExpectedHeader = headerPresentInFirstRow != false && _mustMatchExpectedHeader;
           
        }

        /// <summary>
        /// Constructs the Csv to Object reader instance.  Most of the parameters are optional parameters
        /// with default value.  Override the default value with the custom values
        /// </summary>
        /// <param name="mapper">instance of Csv File to Domain object mapper <see cref="CsvToObjectMapper{T}"/></param>
        /// <param name="fileContent">Content of .csv/txt file</param>
        /// <param name="headerPresentInFirstRow">Does this csv/text file has header row in first line?[default=true]</param>
        /// <param name="mustMatchExpectedHeader">Should this csv file headers match with the header provided in mapper?[default=true]</param>
        /// <param name="ignoreEmptyFile">should empty file be ignored and not marked as error?[default=true]</param>
        /// <param name="ignoreColumnCountMismatch">Should ignore the additional columns if present and not report error?[default=true]</param>
        /// <param name="ignoreDataConversionError">Should ignore the rows failing because of schema/data conversion issues?[default=true]</param>
        public CsvToObjectReader(
            CsvToObjectMapper<T> mapper,
            string fileContent,
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
            this._csvContentLines= fileContent?.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
            );
            this._mustMatchExpectedHeader = mustMatchExpectedHeader;
            this._mapper = mapper;
            this._ignoreDataConversionError = ignoreDataConversionError;
            this._headerPresentInFirstRow = headerPresentInFirstRow;
            this._ignoreEmptyFile = ignoreEmptyFile;
            this._ignoreColumnCountMismatch = ignoreColumnCountMismatch;
            _mustMatchExpectedHeader = headerPresentInFirstRow != false && _mustMatchExpectedHeader;

        }

        /// <summary>
        /// Constructs the Csv to Object reader instance.  Most of the parameters are optional parameters
        /// with default value.  Override the default value with the custom values
        /// </summary>
        /// <param name="fileContentLines">Content of .csv/txt file in a string array</param>
        /// <param name="mapper">instance of Csv File to Domain object mapper <see cref="CsvToObjectMapper{T}"/></param>
        /// <param name="headerPresentInFirstRow">Does this csv/text file has header row in first line?[default=true]</param>
        /// <param name="mustMatchExpectedHeader">Should this csv file headers match with the header provided in mapper?[default=true]</param>
        /// <param name="ignoreEmptyFile">should empty file be ignored and not marked as error?[default=true]</param>
        /// <param name="ignoreColumnCountMismatch">Should ignore the additional columns if present and not report error?[default=true]</param>
        /// <param name="ignoreDataConversionError">Should ignore the rows failing because of schema/data conversion issues?[default=true]</param>
        public CsvToObjectReader(
            string[] fileContentLines,
            CsvToObjectMapper<T> mapper,
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
            this._mustMatchExpectedHeader = mustMatchExpectedHeader;
            this._mapper = mapper;
            this._ignoreDataConversionError = ignoreDataConversionError;
            this._headerPresentInFirstRow = headerPresentInFirstRow;
            this._ignoreEmptyFile = ignoreEmptyFile;
            this._ignoreColumnCountMismatch = ignoreColumnCountMismatch;
            _mustMatchExpectedHeader = headerPresentInFirstRow != false && _mustMatchExpectedHeader;

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
        }

        /// <summary>
        /// Runs series of pre validations on the given CSV file before starting extracting data 
        /// </summary>
        /// <param name="pathToCsv">Path to .csv/other allowed extension file</param>
        private bool PreExtractValidation(string pathToCsv)
        {
            if (pathToCsv == null && _csvContentLines.Length==0) {
                AddError(ErrorCodes.NullPath,"Please supply the file content or provide the path to the file in constructor argument");
                return false;
            }
            else if (!string.IsNullOrEmpty(pathToCsv) && !_fileService.FileExists(pathToCsv)) { 
                AddError(ErrorCodes.PathNotExists,$"{pathToCsv}  does not exists");
                return false;
            }
            else if (!string.IsNullOrEmpty(pathToCsv) && _fileService.PathGetExtension(pathToCsv).ToUpper() != ".CSV")
            { 
                AddError(ErrorCodes.InvalidFileExtension,$"Invalid file extension {_fileService.PathGetExtension(pathToCsv)}. Only .csv is supported ");
                return false;
            }
            try
            {
                if(_csvContentLines.Length == 0)
                    _csvContentLines = _fileService.ReadAllLines(pathToCsv);
            }
            catch (Exception ex)
            {
                AddError(ErrorCodes.CannotReadFile,$"{pathToCsv} Cannot be read\n Exception:{ex.Message}\nStacktrace:{ex.StackTrace}");
                return false;
            }

            if (
                ((_csvContentLines.Length == 0 || 
                 (_csvContentLines.Length ==1 && string.IsNullOrEmpty(_csvContentLines[0].Trim())))
                 && _ignoreDataConversionError == false))
            {
                AddError(ErrorCodes.FileEmpty,"Csv file Content is empty");
                return false;
            }

            if (_headerPresentInFirstRow)//Read the first row as Header
            {
                this.HeaderColumnNamesInCsvFile = SplitLineOfTextIntoCsv.Split(_csvContentLines[0]);
                return IsCsvColumnNamesAsExpectedWithoutDuplicateColumnNames();//Throws Exception if csv columns are not as expected
            }
            return true;
        }

        
        /// <summary>
        /// Takes a line of text and converts into domain object T
        /// </summary>
        /// <param name="lineOfRecordFromCsv">line of text from csv file</param>
        /// <param name="convertedObj">Converted domain object</param>
        /// <returns></returns>
        private bool ConvertCsvRecordToObject(string lineOfRecordFromCsv, out T convertedObj)
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
                ICsvToObjectMap<T> map = this._mapper.ObjectToCsvMapping[property.Name];
                string csvColumnNameFromMap = map.CsvColumnName;
                string columnValue = _csvHeaderAndData[csvColumnNameFromMap];
                try
                {
                    property.SetValue(domainObj, Convert.ChangeType(columnValue, property.PropertyType), null);
                }
                catch (Exception ex)//Handle data conversion failure(if csv data is not compatible with object datatype)
                {
                    convertedObj = default(T);
                    this.ExtractFailedRows.Add(lineOfRecordFromCsv);
                    AddError(ErrorCodes.DataConversionError, $"Cannot Convert data '{columnValue}' for Column name {csvColumnNameFromMap}, column data type {property.PropertyType.Name}\n{ex.Message}\n{ex.StackTrace}");
                    return false;
                }
            }

            convertedObj = domainObj;
            return true;
        }

        
        /// <summary>
        /// Helper method to validate schema and duplicate column names
        /// </summary>
        /// <returns></returns>
        private bool IsCsvColumnNamesAsExpectedWithoutDuplicateColumnNames()
        {
            if (_mustMatchExpectedHeader && _headerPresentInFirstRow)
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
                if (columnsNotMatches == false )
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
            if (PreExtractValidation(_pathToCsv))//If PreExtract Validation is successful, parse the file
            {
                foreach (var line in _csvContentLines.Skip(_headerPresentInFirstRow ? 1 : 0))
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
