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
    public class ServerLogFileId : IServerLogFileId
    {
        public uint FileId { get; set; }
    }
}
