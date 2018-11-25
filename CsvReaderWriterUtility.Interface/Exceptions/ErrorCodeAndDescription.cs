namespace CsvReadWriteUtility.Exceptions
{
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