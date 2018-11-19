namespace LogFileGrowthTracker.Exceptions
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
        CsvColumnNameMissing=105,
        ParameterNull = 106
    }
}