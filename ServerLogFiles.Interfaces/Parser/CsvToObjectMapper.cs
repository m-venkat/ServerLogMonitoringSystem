using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ServerLogMonitorSystem.Exceptions;


namespace ServerLogMonitorSystem.Parser
{

    public class CsvToObjectMap<T>
    {
        public Expression<Func<T>> Property { get; set; }
        public String CsvColumnName { get; set; }
    }

    /// <summary>
    /// Class responsible for adding Mapping between object property and CSV column name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public  class CsvToObjectMapper<T>
    {
        public Dictionary<string, CsvToObjectMap<T>> ObjectToCsvMapping { get; private set; } = new Dictionary<string, CsvToObjectMap<T>>();

        //public void AddMap(Expression<Func<T>> property, string csvColumnName)
        public void AddMap(Func<T> property, string csvColumnName)
        {
            if (property is null)
                throw new LogFileGrowthTrackerException($"property cannot be null", ErrorCodes.ParameterNull);
            if (csvColumnName is null)
                throw new LogFileGrowthTrackerException($"csvColumnName cannot be null", ErrorCodes.ParameterNull);
            
            //var name = ((MemberExpression)property).Member.Name;
            var name = property.Method.Name;
            ObjectToCsvMapping.Remove(name);

            //ObjectToCsvMapping.Add(name, new CsvToObjectMap<T>(){CsvColumnName = csvColumnName, Property = property});
        }

    }


}
