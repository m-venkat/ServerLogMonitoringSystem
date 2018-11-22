using System;

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
        //This is additional property exposed to format the date we expected to persist in csv file
        //This additional property can be avoided by using attribute decorator on top of TimeStamp 
        //attribute in future
        public string TimeStampFormatted => TimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
        public double SizeInBytes { get; set; }
        public double GrowthRateInBytesPerHour { get; set; }
       

    }
}
