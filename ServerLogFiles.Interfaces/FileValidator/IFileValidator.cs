using System;
using System.Collections.Generic;
using System.Text;

namespace ServerLogMonitorSystem.FileValidator
{
    interface IFileValidator
    {
        /// <summary>
        /// Runs validation and return boolean value indicating the validity of file
        /// As part of validation it checks HasValidExtension , HasValidColumnDelimiter, HasValidNumberOfColumns, HasHeaderRow
        /// </summary>
        bool IsFileValid {get;}
        bool HasValidExtension { get; }
        bool HasValidColumnDelimiter{ get; }
        bool HasValidNumberOfColumns { get; }
        bool HasHeaderRow { get; }
        bool HasExpectedHeaderRowCaseSensitive { get; }
        bool HasExpectedHeaderRowCaseInSensitive { get; }

    }
}
