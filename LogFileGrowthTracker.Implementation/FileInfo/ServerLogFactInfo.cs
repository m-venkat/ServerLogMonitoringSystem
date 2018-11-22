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
    public class ServerLogFactInfo : IServerLogFactInfo
    {
        public uint FileId { get; set; }
        public DateTime TimeStamp { get; set; }
        public double SizeInBytes { get; set; }
    }
}
