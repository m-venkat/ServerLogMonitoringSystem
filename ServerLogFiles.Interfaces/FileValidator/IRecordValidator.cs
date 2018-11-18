using System;
using System.Collections.Generic;
using System.Text;

namespace LogFileGrowthTracker.FileValidator
{
    interface IRecordValidator
    {
        /// <summary>
        /// Checks if the record is valid according to the expected schema (no of columns and data types)
        /// </summary>
        bool IsValidRecord { get; }
        /// <summary>
        /// Gets the number of columns in the given record
        /// </summary>
        bool ColumnsCount { get; }
    }
}
