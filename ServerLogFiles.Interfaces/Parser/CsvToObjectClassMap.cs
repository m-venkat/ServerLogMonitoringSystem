using System;
using System.Linq.Expressions;
using System.Text;
using CsvHelper.Configuration;


namespace ServerLogMonitorSystem.Parser
{
    /// <summary>
    /// Encapsulates the inner working of ClassMap<T> and just exposes 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CsvToObjectClassMap<T> : ClassMap<T> { }


}
