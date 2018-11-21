using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ServerLogMonitorSystem.FileInfo;

namespace ServerLogMonitorSystem.DomainModelGenerator
{

    /// <summary>
    /// This interface defines contracts for the Class that Implements custom domain model generation
    /// Implementation class will take two or more domain models as constructor parameter and it will
    /// implement the custom logic to merge/join/aggregate calculate new fields and return the final
    /// type List{T} or T based on the method calls.
    /// e.g. Generate List{ResultDomainObject} by taking List{InputOneDomainObject}
    /// and List{InputTwoDomainObject} and applying custom transformation/aggregation.
    /// In future if a new transformation for another set of domain class is required, new implementation
    /// following this contract should be made.
    /// </summary>
    public class ServerLogFactGrowthInfoGenerator : IDomainModelGenerator<ServerLogFactGrowthInfo>
    {
        private readonly IList<IServerLogFileInfo> _files;
        private readonly IList<IServerLogFactInfo> _fileFacts;
        private  IList<ServerLogFactGrowthInfo> _serverLogFactGrowthInfoList;
        private readonly IList<IList<ServerLogFactGrowthInfo>> _serverLogFactGrowthInfoGroupedByKeyList;
        private const double MilliSecondsInOneHour = 3600000;

        public ServerLogFactGrowthInfoGenerator(IList<IServerLogFileInfo> files, IList<IServerLogFactInfo> fileFacts)
        {
            this._files =
                files ?? throw new ArgumentException($"files argument cannot be null");
            this._fileFacts = fileFacts ?? throw new ArgumentException($"fileFacts argument cannot be null");
            _serverLogFactGrowthInfoList = new List<ServerLogFactGrowthInfo>();
            _serverLogFactGrowthInfoGroupedByKeyList = new List<IList<ServerLogFactGrowthInfo>>();
        }


    /// <summary>
    /// Joins the File and Fact DataSet and produce a result data set that includes columns from
    /// both datasets and also an additional calculated column
    /// </summary>
    /// <returns></returns>
    internal IList<ServerLogFactGrowthInfo> JoinFileAndFactDataSetHorizontally()
        {
            //Join files and facts datasets (horizontal join) to bring columns of both tables into single dataset;
            var res = _fileFacts.Join(_files,
                fileStats => fileStats.FileId,
                files => files.FileId,
                (fileStats, files) => new ServerLogFactGrowthInfo() //Factory pattern DI //TBD
                {
                    FileId = files.FileId,
                    FileName = files.FileName,
                    TimeStamp = fileStats.TimeStamp,
                    SizeInBytes = fileStats.SizeInBytes
                }
            ).OrderBy(o => o.FileId).ThenBy(o => o.TimeStamp);
            return res.ToList();
        }
        /// <summary>
        /// e.g. Generate List{ServerLogFactGrowthInfo} by taking List{IServerLogFileInfo}
        /// and List{IServerLogFactInfo} as constructor argument and applying custom transformation/aggregation.
        /// </summary>
        /// <returns>IList{T}</returns>
        public IList<ServerLogFactGrowthInfo> GenerateList()
        {
            throw new NotSupportedException(@"This method is not applicable for this implementation");
        }

        /// <summary>
        /// Generates the Sliced DataSet by given Key column/property (group by)
        /// </summary>
        /// <returns>IList<IList{T}></IList></returns>
        //public IList<List<ServerLogFactGrowthInfo>> GenerateSlicedList<TKey>(
        //    Expression<Func<ServerLogFactGrowthInfo, TKey>> propertyToSlice)

        public IList<List<ServerLogFactGrowthInfo>> GenerateSlicedList()
        {
            _serverLogFactGrowthInfoList = JoinFileAndFactDataSetHorizontally();
            _serverLogFactGrowthInfoList = CalculateAndAdd_MinutesSinceLastLogCreatedForThisFile_GrowthRateInBytesPerHour(_serverLogFactGrowthInfoList);
            var res = _serverLogFactGrowthInfoList.GroupBy(t => t.FileId).Select(grp => grp.ToList()).ToList();
            foreach (var list in res)
                list.RemoveAt(0);//Remove the first Element [First element will not have previous entry to compare growth]
            return res;
        }


        /// <summary>
        /// Joins the File and Fact DataSet and produces a result data set that includes columns from
        /// both datasets and also an additional calculated column
        /// </summary>
        /// <returns></returns>
        internal IList<ServerLogFactGrowthInfo> CalculateAndAdd_MinutesSinceLastLogCreatedForThisFile_GrowthRateInBytesPerHour(IList<ServerLogFactGrowthInfo> growthInfoDataSet)
        {
            ServerLogFactGrowthInfo prevRecord = null;
            foreach (var record in growthInfoDataSet)
            {
                if (prevRecord != null)
                {
                    double fileGrowthInBytes = record.SizeInBytes - prevRecord.SizeInBytes;
                    double milliSecondsSinceLastLogCreatedForThisFile =
                        record.TimeStamp.Subtract(prevRecord.TimeStamp).TotalMilliseconds;
                    record.MilliSecondsSinceLastLogCreatedForThisFile = milliSecondsSinceLastLogCreatedForThisFile;
                    record.GrowthRateInBytesPerHour =
                        Math.Round(CalculateLogFileGrowthPerHourInBytes(milliSecondsSinceLastLogCreatedForThisFile,
                            fileGrowthInBytes),1);
                }
                prevRecord = record;
            }

            return growthInfoDataSet;
        }


        /// <summary>
        /// Helper/formula to calculate the file growthrate per hour
        /// </summary>
        /// <param name="milliSecondsSinceLastLogCreatedForThisFile">Time stamp difference in milliseconds between two files</param>
        /// <param name="fileGrowthInBytes">File size grown in bytes</param>
        /// <returns></returns>
        internal double CalculateLogFileGrowthPerHourInBytes( double milliSecondsSinceLastLogCreatedForThisFile,
            double fileGrowthInBytes)
        {

            return (fileGrowthInBytes / milliSecondsSinceLastLogCreatedForThisFile) * MilliSecondsInOneHour;
        }
            

        /// <summary>
        /// Not Supported for this scenario
        /// </summary>
        /// <returns>T</returns>
        public ServerLogFactGrowthInfo Generate()
        {
            throw new NotSupportedException(@"This method is not applicable for this implementation");
        }
    }
}
