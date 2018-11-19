﻿namespace ServerLogMonitorSystem.FileInfo
{
    /// <summary>
    ///This Interface represents the file shape/schema of transformed/generated output csv file
    /// that contains GrowthRateInBytesPerHour column.
    /// If any columns need to be added to output, it will go here in this
    /// interface definition.
    /// </summary>
    public interface IServerLogFactGrowthInfo : IServerLogFileInfo, IServerLogFactInfo
    {
         uint GrowthRateInBytesPerHour { get; set; }
    }
}
