namespace ServerLogGrowthTracker.FileInfo
{

  public class ServerLogFileInfo : IServerLogFileInfo
    {
        public uint FileId { get; set; }
        public string FileName { get; set; }
    }
}
