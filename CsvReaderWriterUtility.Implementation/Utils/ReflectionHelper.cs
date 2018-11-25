using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CsvReadWriteUtility.Utils;

namespace CsvReadWriteUtility.Utils
{
    public class ReflectionHelper<T> : IReflectionHelper<T> where T : class  
    {
        /// <summary>
        /// Reflects the given T and extracts the property metadata and value for all columns
        /// </summary>
        /// <param name="tmodelObj"></param>
        /// <returns></returns>
        public IList<IReflectedPropertyInfo> GetReflectedPropertyInfo(T tmodelObj)
        {
            var reflectedPropertyColumns = new List<ReflectedPropertyInfo>();
            //Getting Type of Generic Class Model
            Type tModelType = tmodelObj.GetType();

            //We will be defining a PropertyInfo Object which contains details about the class property 
            PropertyInfo[] arrayPropertyInfos = tModelType.GetProperties();

            //Now we will loop in all properties one by one to get value
            foreach (PropertyInfo property in arrayPropertyInfos)
            {
                reflectedPropertyColumns.Add(new ReflectedPropertyInfo()
                {
                    PropertyDataType = property.PropertyType.Name,
                    PropertyName = property.Name,
                    PropertyValue = property.GetValue(tmodelObj).ToString()
                });
            }
            return reflectedPropertyColumns.Cast<IReflectedPropertyInfo>().ToList();
        }
    }
}
