namespace ServerLogMonitorSystem.Exceptions
{
    /// <summary>
    /// All Api/method level custom Exceptions will be thrown as LogFileGrowthTrackerException.
    /// </summary>
    public class LogFileGrowthTrackerException : BaseException
    {
        public LogFileGrowthTrackerException(string message,  ErrorCodes code = ErrorCodes.Undefined, string stackTrace ="") : base(message)
        {
            _stackTrace = stackTrace;
            ErrorCode = code;
        }
    }
}