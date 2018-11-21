using System.Collections.Generic;

namespace CsvReadWriteUtility.Utils
{
    public interface IReflectionHelper<in T> where T : class
    {
        /// <summary>
        /// Reflects the given T and extracts the property metadata and value for all columns
        /// </summary>
        /// <param name="tmodelObj"></param>
        /// <returns></returns>
        IList<IReflectedPropertyInfo> GetReflectedPropertyInfo(T tmodelObj);
    }
}
