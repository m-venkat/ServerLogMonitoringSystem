using System;
using System.Collections.Generic;

namespace ServerLogGrowthTracker.FileInfo
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
        public DateTime TimeStamp { get ; set; }
        public string TimeStampFormatted => TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
        public double SizeInBytes { get; set; }
        public double GrowthRateInBytesPerHour { get; set; }
        
        public double MilliSecondsSinceLastLogCreatedForThisFile { get; set; }

        public IServerLogFactGrowthInfo GetInstance()
        {
            return new ServerLogFactGrowthInfo();
        }

    }
}
