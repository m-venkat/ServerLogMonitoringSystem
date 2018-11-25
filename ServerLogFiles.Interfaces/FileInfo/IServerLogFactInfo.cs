using System;

namespace ServerLogGrowthTracker.FileInfo
{
    /// <summary>
    /// Interface/Contract that defines the shape of FileStat.csv
    /// File content will be as given below
    ///FileID Timestamp   SizeInBytes
    ///1	3/25/2015 23:00	4245143
    ///1	3/25/2015 23:55	4276852
    /// </summary>
    public interface IServerLogFactInfo : IServerLogFileId
    {
        /// <summary>
        /// Timestamp of Log File with Millisecond Permission
        /// </summary>
        DateTime TimeStamp { get; set; }

        /// <summary>
        /// Timestamp of Log File with Millisecond Permission
        /// </summary>
        double SizeInBytes { get; set; }
    }
}
