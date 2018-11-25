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
    }
}