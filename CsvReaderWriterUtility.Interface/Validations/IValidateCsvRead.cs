namespace CsvReadWriteUtility.Validations
{
    public interface IValidateCsvRead
    {
        bool HasValidExtension { get; }
        bool IsFileReadable { get; }
        bool HasColumnsAsExpected { get; }
    }
}
