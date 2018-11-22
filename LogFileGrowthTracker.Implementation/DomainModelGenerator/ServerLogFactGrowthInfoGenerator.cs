using System;
using System.Collections.Generic;
using System.Linq;
using ServerLogGrowthTracker.FileInfo;

namespace ServerLogGrowthTracker.DomainModelGenerator
{

    /// <summary>
    /// This interface defines contracts for the class that Implements custom domain model generation.
    /// Domain Model is a strongly typed C# class that representes a revcord in a csv file.
    /// IList{DomainModel} will represent complete content/all lines of a csv file
    /// 
    /// Each column in csv file will map to a property of C# class (domain model)
    /// Implementation class will take two or more domain models as constructor parameter and it will
    /// implement the custom logic to merge/join/aggregate calculate new fields and return the final
    /// type List{T} or T based on the method calls.
    /// 
    /// e.g. Generate List{ResultDomainObject} by taking List{InputOneDomainObject}
    /// and List{InputTwoDomainObject} and applying custom transformation/aggregation.
    /// 
    /// In future if a new transformation for another set of domain class is required, new implementation
    /// following this contract should be made.
    /// </summary>
    public class ServerLogFactGrowthInfoGenerator<T> : IDomainModelGenerator<T> where T : IServerLogFactGrowthInfo
    {
        private readonly IList<IServerLogFileInfo> _files;
        private readonly IList<IServerLogFactInfo> _fileFacts;
        private IList<T> _serverLogFactGrowthInfoList;
        private readonly IList<IList<ServerLogFactGrowthInfo>> _serverLogFactGrowthInfoGroupedByKeyList;
        private const double MilliSecondsInOneHour = 3600000;

        public ServerLogFactGrowthInfoGenerator(IList<IServerLogFileInfo> files, IList<IServerLogFactInfo> fileFacts)
        {
            this._files =
                files ?? throw new ArgumentException($"files argument cannot be null");
            this._fileFacts = fileFacts ?? throw new ArgumentException($"fileFacts argument cannot be null");
            _serverLogFactGrowthInfoList = new List<T>();
            _serverLogFactGrowthInfoGroupedByKeyList = new List<IList<ServerLogFactGrowthInfo>>();
        }


        /// <summary>
        /// Joins the File and Fact DataSet and produce a result data set that includes columns from
        /// both datasets and also an additional calculated column
        /// </summary>
        /// <returns></returns>
        internal IList<T> JoinFileAndFactDataSetHorizontally()
        {
            //Join files and facts datasets (horizontal join) to bring columns of both tables into single dataset;
            var res = _fileFacts.Join(_files,
                fileStats => fileStats.FileId,
                files => files.FileId,
                (fileStats, files) =>
                {
                    var domainObj = (T)Activator.CreateInstance(typeof(T));
                    domainObj.FileId = files.FileId;
                    domainObj.FileName = files.FileName;
                    domainObj.TimeStamp = fileStats.TimeStamp;
                    domainObj.SizeInBytes = fileStats.SizeInBytes;
                    return domainObj;

                }).OrderBy(o => o.FileId).ThenBy(o => o.TimeStamp);
            return res.ToList();
        }
        /// <summary>
        /// e.g. Generate List{ServerLogFactGrowthInfo} by taking List{IServerLogFileInfo}
        /// and List{IServerLogFactInfo} as constructor argument and applying custom transformation/aggregation.
        /// </summary>
        /// <returns>IList{T}</returns>
        public IList<T> GenerateList()
        {
            throw new NotSupportedException(@"This method is not applicable for this implementation");
        }

        /// <summary>
        /// Generates the Sliced DataSet by given Key column/property (group by)
        /// </summary>
        /// <returns>IList<IList{T}></IList></returns>
        //public IList<List<ServerLogFactGrowthInfo>> GenerateSlicedList<TKey>(
        //    Expression<Func<ServerLogFactGrowthInfo, TKey>> propertyToSlice)

        public IList<List<T>> GenerateSlicedList()
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
        internal IList<T> CalculateAndAdd_MinutesSinceLastLogCreatedForThisFile_GrowthRateInBytesPerHour(IList<T> growthInfoDataSet)
        {
            IServerLogFactGrowthInfo prevRecord = null;
            foreach (var record in growthInfoDataSet)
            {
                if (prevRecord != null)
                {
                    double fileGrowthInBytes = record.SizeInBytes - prevRecord.SizeInBytes;
                    double milliSecondsSinceLastLogCreatedForThisFile =
                        record.TimeStamp.Subtract(prevRecord.TimeStamp).TotalMilliseconds;
                   // record.MilliSecondsSinceLastLogCreatedForThisFile = milliSecondsSinceLastLogCreatedForThisFile;
                    record.GrowthRateInBytesPerHour =
                        Math.Round(CalculateLogFileGrowthPerHourInBytes(milliSecondsSinceLastLogCreatedForThisFile,
                            fileGrowthInBytes), 1);
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
        internal double CalculateLogFileGrowthPerHourInBytes(double milliSecondsSinceLastLogCreatedForThisFile,
            double fileGrowthInBytes)
        {

            return (fileGrowthInBytes / milliSecondsSinceLastLogCreatedForThisFile) * MilliSecondsInOneHour;
        }


        /// <summary>
        /// Not Supported for this scenario
        /// </summary>
        /// <returns>T</returns>
        public T Generate()
        {
            throw new NotSupportedException(@"This method is not applicable for this implementation");
        }
    }
}
