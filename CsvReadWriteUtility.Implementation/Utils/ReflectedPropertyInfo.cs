using CsvReadWriteUtility.Utils;

namespace CsvReadWriteUtility.Utils
{
    public class ReflectedPropertyInfo : IReflectedPropertyInfo
    {
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
        public string PropertyDataType { get; set; }

    }
}
