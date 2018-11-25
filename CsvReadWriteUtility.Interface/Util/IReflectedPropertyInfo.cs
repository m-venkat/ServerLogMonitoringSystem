using System;
using System.Text;

namespace CsvReadWriteUtility.Utils
{
    public interface IReflectedPropertyInfo
    {
        string PropertyName { get; set; }
        string PropertyValue { get; set; }
        string PropertyDataType { get; set; }
    }
}
