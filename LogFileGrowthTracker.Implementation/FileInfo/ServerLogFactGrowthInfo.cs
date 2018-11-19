using System;
using System.Collections.Generic;

namespace ServerLogMonitorSystem.FileInfo
{
    /// <summary>
    ///This Interface represents the file shape/schema of transformed/generated output csv file
    /// that contains GrowthRateInBytesPerHour column.
    /// If any columns need to be added to output, it will go here in this
    /// interface definition.
    /// </summary>
    public class ServerLogFactGrowthInfo : IServerLogFactGrowthInfo
    {
        public uint FileId { get; set; }
        public string FileName { get; set; }
        public DateTime TimeStamp { get; set; }
        public uint SizeInBytes { get; set; }
        public uint GrowthRateInBytesPerHour { get; set; }
        public uint MinutesSinceLastLogCreated { get; set; }
    }
}
