using System;
using System.Collections.Generic;
using System.Text;

namespace LogFileGrowthTracker.Exceptions
{
    public enum ErrorCodes
    {
        InvalidFileExtension = 100,
        FileEmpty = 101,
        PathNotExists =102,
        CannotReadFile = 103,
        Undefined = 0,
        NullPath = 104,
        CsvColumnNameMissing=105
    }

    public class BaseException : Exception {
    protected string _stackTrace = string.Empty;
        public ErrorCodes ErrorCode { get; protected set; } = ErrorCodes.Undefined;
        public BaseException(string message) : base(message) { }
        public override string StackTrace => string.IsNullOrEmpty(_stackTrace) ? base.StackTrace : _stackTrace;
    }

    public class LogFileGrowthTrackerException : BaseException
    {
        public LogFileGrowthTrackerException(string message,  ErrorCodes code = ErrorCodes.Undefined, string stackTrace ="") : base(message)
        {
            _stackTrace = stackTrace;
            ErrorCode = code;
        }
    }
    
}
