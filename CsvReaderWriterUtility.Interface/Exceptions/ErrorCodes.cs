namespace CsvReadWriteUtility.Exceptions
{
    /// <summary>
    /// Application level Error codes if client needs to handle it by catching and switching the errror codes.
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
        DataConversionError = 109,
        CannotWriteFileOrDirectory=110,
        FileNameListCountNotMatches =111,
        CsvWithoutHeaderNotSupported = 112,
        CsvHeaderMustMatchWithMapping = 113,
        CannotOverWrite =114,
        ObjectToCsvConvertionError=115
    }
}