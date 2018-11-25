namespace CsvReadWriteUtility.Exceptions
{
    /// <summary>
    /// All Api/method level custom Exceptions will be thrown as CsvReadWriteException.
    /// </summary>
    public class CsvReadWriteException : BaseException
    {
        public CsvReadWriteException(string message,  ErrorCodes code = ErrorCodes.Undefined, string stackTrace ="") : base(message)
        {
            _stackTrace = stackTrace;
            ErrorCode = code;
        }
        public CsvReadWriteException(ErrorCodes code = ErrorCodes.Undefined, string stackTrace = "") : base(GetFormattedErrorMessage(code))
        {
            _stackTrace = stackTrace;
            ErrorCode = code;
        }

        private static string GetFormattedErrorMessage(ErrorCodes errorCode)
        {
            switch (errorCode)
            {
                case ErrorCodes.CsvWithoutHeaderNotSupported:
                    return "Csv without header is not supported in this version, Please ensure header is present in the first line of CSV file";
                case ErrorCodes.CsvHeaderMustMatchWithMapping:
                    return "Csv header column should match with the supplied Mapper information (Future versions may handle mismatch)";
                case ErrorCodes.FileEmpty:
                    return "Csv File is empty";

                default:
                {
                    return "";
                }
            }
        }
    }
}