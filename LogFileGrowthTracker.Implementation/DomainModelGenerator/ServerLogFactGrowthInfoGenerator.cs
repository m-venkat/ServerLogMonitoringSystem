using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using ServerLogGrowthTracker.FileInfo;

[assembly: InternalsVisibleTo("ServerLogMonitoringSystem.Tests")]
namespace ServerLogGrowthTracker.DomainModelGenerator
{

    /// <summary>
    /// This interface defines contracts for the class that Implements custom domain model generation.
    /// Domain Model is a strongly typed C# class that represents a record in a csv file.
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
        /// Slice all the records by grouping on a particular column (e.g. File ID ) and creates multiple data sets
        /// Each data set will be persisted/saved to CSV file
        /// </summary>
        /// <returns>IList<IList{T}></IList></returns>
        public IList<List<T>> GenerateSlicedList()
        {
            _serverLogFactGrowthInfoList = JoinFileAndFactDataSetHorizontally();
            _serverLogFactGrowthInfoList = CalculateAndSetGrowthRateInBytesPerHour(_serverLogFactGrowthInfoList);
            var res = _serverLogFactGrowthInfoList.GroupBy(t => t.FileId).Select(grp => grp.ToList()).ToList();
            foreach (var list in res)
                list.RemoveAt(0);//Remove the first Element [First element will not have previous entry to compare growth]
            return res;
        }


        /// <summary>
        /// For each record in the list it calculates the file growth rate and sets it to the appropriate property
        /// </summary>
        /// <returns>IList{T}</returns>
        internal IList<T> CalculateAndSetGrowthRateInBytesPerHour(IList<T> growthInfoDataSet)
        {
            IServerLogFactGrowthInfo prevRecord = null;
            foreach (var record in growthInfoDataSet)
            {
                if (prevRecord != null)
                {
                    record.GrowthRateInBytesPerHour = Math.Round(CalculateLogFileGrowthPerHourInBytes(prevRecord.SizeInBytes,
                        record.SizeInBytes, prevRecord.TimeStamp, record.TimeStamp),1);
                }
                prevRecord = record;
            }

            return growthInfoDataSet;
        }


        /// <summary>
        /// Helper function to calculate the file growth rate based on the timestamp and file size
        /// </summary>
        /// <param name="lastFileSize"></param>
        /// <param name="currentFileSize"></param>
        /// <param name="lastTimeStamp"></param>
        /// <param name="currentTimeStamp"></param>
        /// <returns></returns>
        internal double CalculateLogFileGrowthPerHourInBytes(double lastFileSize, double currentFileSize,DateTime lastTimeStamp, DateTime currentTimeStamp)
        {
            //If Current File size is less than or equal to last file size, it indicates no growth in file, hence return 0
            if (currentFileSize <= lastFileSize) return 0;

            double fileGrowthInBytes = currentFileSize - lastFileSize;
            double milliSecondsSinceLastLogCreatedForThisFile =
                currentTimeStamp.Subtract(lastTimeStamp).TotalMilliseconds;
            
            //If Current Timestamp is earlier than or equal to  last file timestamp, it indicates no growth in file, hence return 0
            if (milliSecondsSinceLastLogCreatedForThisFile <= 0) return 0;//To Avoid divide by Zero error

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
