namespace CsvReadWriteUtility.Exceptions
{
    /// <summary>
    /// Error Codes, will grow based on scenarios.
    /// </summary>
    public enum ErrorCodes
    {
        InvalidFileExtension = 100,
        FileEmpty = 101,
        PathNotExists =102,
        CannotReadFile = 103,
        Undefined = 0,
        NullPath = 104,
        CsvColumnNameNotFound=105,
        ParameterNull = 106,
        ColumnCountMismatch = 107,
        DuplicateColumnNames = 108,
        DataConversionError = 109
    }

    public class ErrorCodeAndDescription
    {
        public ErrorCodes ErrorCode { get;  set; }
        public string ErrorDescription { get;  set; }

        public void AddErrorCodeAndDescription(ErrorCodes errorCode, string errorDescription)
        {
            this.ErrorCode = errorCode;
            this.ErrorDescription = errorDescription;
        }
    }
}