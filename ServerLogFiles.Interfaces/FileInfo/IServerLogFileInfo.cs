namespace ServerLogGrowthTracker.FileInfo
{

    /// <summary>
    /// Interface/Contract that defines the shape of Files.csv
    /// File content will be as given below
    /// ID	Name
    ///1	c:\program files\sql server\master.mdf
    ///2	c:\program files\sql server\master.ldf
    /// </summary>
    public interface IServerLogFileInfo : IServerLogFileId
    {
        /// <summary>
        /// holds log file name e.g. c:\program files\sql server\master.mdf
        /// </summary>
        string FileName { get; set; }
    }
}
