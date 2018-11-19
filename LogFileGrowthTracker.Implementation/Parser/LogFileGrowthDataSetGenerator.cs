using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogFileGrowthTracker.Exceptions;
using LogFileGrowthTracker.FileInfo;

namespace LogFileGrowthTracker.Parser
{
    public class LogFileGrowthDataSetGenerator
    {
        private List<ServerLogFileInfo> _files = new List<ServerLogFileInfo>();
        private List<ServerLogFactInfo> _fileFacts = new List<ServerLogFactInfo>();
        private List<ServerLogFactGrowthInfo> _serverLogFactGrowthInfoList  = new List<ServerLogFactGrowthInfo>();
        public LogFileGrowthDataSetGenerator(List<ServerLogFileInfo> files, List<ServerLogFactInfo> fileFacts)
        {
            this._files = files ?? throw new LogFileGrowthTrackerException($"files argument cannot be null",ErrorCodes.ParameterNull, "");
            this._fileFacts = fileFacts ?? throw new LogFileGrowthTrackerException($"fileFacts argument cannot be null",ErrorCodes.ParameterNull, "");
        }
        

        public IEnumerable<IServerLogFactGrowthInfo> GenerateLogFileGrowthDataSet()
        {
            //Join files and facts datasets (horizontal join) to bring columns of both tables into single dataset;
            return  _fileFacts.Join(_files,
                fileStats => fileStats.FileId,
                files => files.FileId,
                (fileStats,files ) => new ServerLogFactGrowthInfo //Factory pattern DI //TBD
                {
                    FileId = files.FileId,
                    FileName = files.FileName,
                    TimeStamp = fileStats.TimeStamp,
                    SizeInBytes = fileStats.SizeInBytes
                }
            ).OrderBy(o=> o.FileId).ThenBy(o=> o.TimeStamp);

            //return results;
        }
    }

}
