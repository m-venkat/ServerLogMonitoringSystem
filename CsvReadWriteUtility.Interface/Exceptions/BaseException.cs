using System;
using System.Collections.Generic;
using System.Text;

namespace CsvReadWriteUtility.Exceptions
{
    /// <summary>
    /// Base Exception for LogFileGrowthTracker API's each application API level exception will have ErrorCode
    /// to programmatically/gracefully handle the error based on error codes.
    /// </summary>
    public class BaseException : Exception {
    protected string _stackTrace = string.Empty;
        public ErrorCodes ErrorCode { get; protected set; } = ErrorCodes.Undefined;
        public BaseException(string message) : base(message) { }
        public override string StackTrace => string.IsNullOrEmpty(_stackTrace) ? base.StackTrace : _stackTrace;
    }
}
