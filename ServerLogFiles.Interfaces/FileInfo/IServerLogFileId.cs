using System;

namespace ServerLogGrowthTracker.FileInfo
{

    /// <summary>
    /// Interface/Contract for FileId that is composed in various other child Interfaces
    /// (e.g. IServerLogFactInfo, IServerLogFileInfo etc)
    /// FileId is defined in this interface because to avoid repeating this field in each
    /// other interface.  If we need to change the data type or associated logic, changes
    /// will go into this file.
   /// </summary>
    public interface IServerLogFileId
    {
        /// <summary>
        /// unit - File ID will be always an positive integer so chosen uInt data type. Holds up to 4 billion
        /// If the maximum FileID is going to be less than 65,535 we can choose ushort as the data type
        /// </summary>
        uint FileId { get; set; }
        
    }
}
