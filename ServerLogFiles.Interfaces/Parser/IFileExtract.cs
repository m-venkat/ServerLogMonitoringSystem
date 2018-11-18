using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using CsvHelper.Configuration;


namespace LogFileGrowthTracker.Parser
{
    public interface IExtractCsvToObject<T>
    {
        bool IgnoreDataConversionErrors { get; set; }
        IReaderConfiguration Configuration { get; }
        IEnumerable<T> Extract();
        IEnumerable<T> ExtractFailedRecords();
    }

    //public interface ICsvToObjectMap<T>
    //{
    //    Expression<Func<T>> PropertyExpression { get; set; }
    //    string ColumnNameInCsv { get; set; }
    //    string PropertyColumnName { get; set; }
    //}

    //public interface ICsvToObjectMapper<T>
    //{
    //    void AddMapping(Expression<Func<T>> objProperty, string columnNameInCsvFile);
    //    Dictionary<string, ICsvToObjectMap<T>> CsvToObjectMap { get; }
    //}

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class  CsvToObjectMapper<T> : ClassMap<T>
    {
        //Encapsulating ClassMap<T> to outside world and exposing Library specific class.  This can be abstracted
        //and changed in future
    }
}
