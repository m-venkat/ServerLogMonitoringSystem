namespace ServerLogMonitorSystem.Exceptions
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
        DuplicateColumnNames = 108
    }

    public class ErrorCodeAndDescription
    {
        public ErrorCodes ErrorCode { get; internal set; }
        public string ErrorDescription { get; internal set; }

        public void AddErrorCodeAndDescription(ErrorCodes errorCode, string errorDescription)
        {
            this.ErrorCode = errorCode;
            this.ErrorDescription = errorDescription;
        }
    }
}