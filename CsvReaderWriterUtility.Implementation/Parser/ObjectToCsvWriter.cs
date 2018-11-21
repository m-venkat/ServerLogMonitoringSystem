using System;
using System.Collections.Generic;
using System.Text;
using CsvReadWriteUtility.Exceptions;

namespace CsvReadWriteUtility.Parser
{
    
    /// <summary>
    /// Contract for the object that implements functionality writing object to CSV file .
    /// This Interface provides methods to write the object data into CSV file
    /// This Interface is written in generic way to handle any domain object and csv shape.
    /// CSV to Object mapper should be passed as constructor argument to map the CSV fields with Object property
    /// Fields that are part of mapping will be saved in CSV file and rest of the columns will be ignored.
    /// refer <see cref="ICsvToObjectMapper{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectToCsvWriter<T> : IObjectToCsvWriter<T>
    {
       
        public IList<ErrorCodeAndDescription> ErrorsOccured { get; private set; }

        public bool HasError { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="objecListToPersistAsCsv"></param>
        public ObjectToCsvWriter(List<T> objecListToPersistAsCsv)
        {

        }



        /// <summary>
        /// Writes the object to CSV File
        /// </summary>
        /// <returns></returns>
        public bool Write()
        {
            return true;
        }


        /// <summary>
        /// Writes the object to CSV File, Overwrite if the file already Exists
        /// </summary>
        /// <returns></returns>
        public bool Write(bool overwrite)
        {
            return false;
        }

    }
}
