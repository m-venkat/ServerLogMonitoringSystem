using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ServerLogMonitorSystem.FileInfo;
using ServerLogMonitorSystem.Exceptions;
using ServerLogMonitorSystem.Utils;

namespace ServerLogMonitorSystem.Parser
{
    
    public class CsvToObjectReader<T> : ICsvToObjectReader<T>
    {
        #region Private Variables
        private string _pathToCsv = string.Empty;
        private string[] _csvContentLines ;
        private IList<ErrorCodes> _validationErrors = new List<ErrorCodes>();
        private readonly CsvToObjectMapper<T> _mapper ;
        private readonly bool _ignoreDataConversionError ;
        private readonly bool _headerPresentInFirstRow;
        private readonly bool _mustMatchExpectedHeader;
        private readonly bool _ignoreColumnCountMismatch;
        private bool _ignoreEmptyFile;
        #endregion

        #region Public Properties
        public string[] HeaderColumnNamesInCsvFile { get; private set; }
        #endregion


        /// <summary>
        /// Constructs the Csv to Object reader instance.  Most of the parameters are optional parameters
        /// with default value.  Override the default value with the custom values
        /// </summary>
        /// <param name="pathToCsv">Complete file path to the .csv/txt file</param>
        /// <param name="mapper">instance of Csv File to Domain object mapper <see cref="CsvToObjectMapper{T}"/></param>
        /// <param name="headerPresentInFirstRow">Does this csv/text file has header row in first line?</param>
        /// <param name="mustMatchExpectedHeader">Should this csv file headers match with the header provided in mapper?</param>
        /// <param name="ignoreEmptyFile">should empty file be ignored and not marked as error?</param>
        /// <param name="ignoreColumnCountMismatch">Should ignore the additional columns if present and not report error?</param>
        /// <param name="ignoreDataConversionError">Should ignore the rows failing because of schema/data conversion issues?</param>
        public CsvToObjectReader(
                                string pathToCsv,
                                CsvToObjectMapper<T> mapper = null,
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
            this._mustMatchExpectedHeader = mustMatchExpectedHeader;
            this._mapper = mapper;
            this._ignoreDataConversionError = ignoreDataConversionError;
            this._headerPresentInFirstRow = headerPresentInFirstRow;
            this._ignoreEmptyFile = ignoreEmptyFile;
            this._ignoreColumnCountMismatch = ignoreColumnCountMismatch;
            _mustMatchExpectedHeader = headerPresentInFirstRow != false && _mustMatchExpectedHeader;
           
        }

        /// <summary>
        /// Runs series of validations on the given CSV file before starting extracting data 
        /// </summary>
        /// <param name="pathToCsv">Path to .csv/other allowed extension file</param>
        private bool PreExtractValidation(string pathToCsv)
        {
            if (pathToCsv == null) { 
                _validationErrors.Add(ErrorCodes.NullPath);
                return false;
            }
            else if (!File.Exists(pathToCsv)) { 
                _validationErrors.Add(ErrorCodes.PathNotExists);
                return false;
            }
            else if (Path.GetExtension(pathToCsv).ToUpper() != ".CSV")
            { 
                _validationErrors.Add(ErrorCodes.InvalidFileExtension);
                return false;
            }
            try
            {
                _csvContentLines = File.ReadAllLines(pathToCsv);
            }
            catch (Exception ex)
            {
                _validationErrors.Add(ErrorCodes.CannotReadFile);
                return false;
            }

            if (
                ((_csvContentLines.Length == 0 || 
                 (_csvContentLines.Length ==1 && string.IsNullOrEmpty(_csvContentLines[0].Trim())))
                 && _ignoreDataConversionError == false))
            {
                _validationErrors.Add(ErrorCodes.FileEmpty);
                return false;
            }
            return true;
        }

        
        


       
        public bool Read(out IList<ErrorCodes> validationErrors, out IEnumerable<T> domainObjects)
        {
            domainObjects = null;
            validationErrors = _validationErrors;
            List<T> convertedObjects = new List<T>();
            if (PreExtractValidation(_pathToCsv))//If PreExtract Validation is sucessfull, parse the file
            { 
                if(_csvContentLines.Length == 0)
                {
                    _validationErrors.Add(ErrorCodes.FileEmpty);
                    return false;
                }

                if (!ValidateCsvHeader()) //Validate Header 
                {
                    return false;
                }
                foreach (var line in _csvContentLines.Skip(_headerPresentInFirstRow ? 1 : 0 ))
                {
                    T convertedObj;
                    ConvertCsvRecordToObject(line, out convertedObj);
                    convertedObjects.Add(convertedObj);
                }
            }
            else
            {
                return false;
            }
            validationErrors = _validationErrors;
            domainObjects= convertedObjects.AsEnumerable();
            return true;
        }

        private bool ConvertCsvRecordToObject(string lineOfRecordFromCsv, out T convertedObj)
        {
            string[] columnData = SplitTextRowIntoCsv.Split(lineOfRecordFromCsv);
            Dictionary<string,string> csvHeaderAndData = new Dictionary<string, string>();
            
            for (ushort i=0;i<=HeaderColumnNamesInCsvFile.Length;i++)
            {
                if(i < HeaderColumnNamesInCsvFile.Length && i < columnData.Length )
                    csvHeaderAndData.Add(HeaderColumnNamesInCsvFile[i], columnData[i]);
            }

            if (_ignoreColumnCountMismatch == false && columnData.Length != HeaderColumnNamesInCsvFile.Length)
            {
                _validationErrors.Add(ErrorCodes.ColumnCountMismatch);
                convertedObj = default(T);
                return false;
            }

            T domainObj = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] properties = domainObj.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                //Get Each property name from list of properties
                CsvToObjectMap<T> map = this._mapper.ObjectToCsvMapping[property.Name];
                string csvColumnNameFromMap = map.CsvColumnName;
                string columnValue = csvHeaderAndData[csvColumnNameFromMap];
                property.SetValue(domainObj, Convert.ChangeType(columnValue, property.PropertyType), null);
            }

            convertedObj = domainObj;
            return true;
        }

        /// <summary>
        /// Private Method to Validate the CSV Header file and throw Exceptions
        /// </summary>
        private bool ValidateCsvHeader()
        {
            //Get the Header Row of CSV File
            if (_headerPresentInFirstRow)//Read the first row as Header
            {
                this.HeaderColumnNamesInCsvFile = SplitTextRowIntoCsv.Split(_csvContentLines[0]);
                return IsCsvColumNamesAsExpectedWithoutDuplicateColumnNames();//Throws Exception if csv columns are not as expected
            }

            return true;
        }

        /// <summary>
        /// Are the columns in Csv file is as expected by the mapping input parameters
        /// </summary>
        private bool IsCsvColumNamesAsExpectedWithoutDuplicateColumnNames()
        {
            if (_mustMatchExpectedHeader && _headerPresentInFirstRow)
            {
                bool columnsNotMatches = true;
                foreach (var columnName in HeaderColumnNamesInCsvFile)
                {
                    var obj = _mapper.ObjectToCsvMapping.Where(item =>
                            item.Value.CsvColumnName.Trim().ToUpper() == columnName.Trim().ToUpper())
                        .Select(e => (KeyValuePair<string, CsvToObjectMap<T>>?)e)
                        .FirstOrDefault();
                    if (obj == null)
                        columnsNotMatches = false;
                }
                if (columnsNotMatches == false )
                {
                    this._validationErrors.Add(ErrorCodes.CsvColumnNameNotFound);
                    return false;
                }

                if (HeaderColumnNamesInCsvFile.Select(i => i.Trim().ToUpper()).Distinct().Count() !=
                    HeaderColumnNamesInCsvFile.Length)
                {
                    this._validationErrors.Add(ErrorCodes.DuplicateColumnNames);
                    return false;
                }

            }
            return true;
        }

       

        public IEnumerable<IEnumerable<T>> SliceDataSetByKey(Expression<Func<T>> keyProperty)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string[]> ExtractFailedRecords()
        {
            throw new NotImplementedException();
        }
    }
}
