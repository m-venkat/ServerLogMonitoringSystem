using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CsvReadWriteUtility.Exceptions;
using CsvReadWriteUtility.Parser;
using CsvReadWriteUtility.Utils;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ServerLogGrowthTracker.DomainModelGenerator;
using ServerLogGrowthTracker.FileInfo;
using Xunit;
namespace ServerLogMonitoringSystem.Tests
{
    public class CsvToObjectReaderTests :IDisposable
    {
        private IFileService _fileServiceMock;
        private IFileService _fileServiceReal;
        private CsvToObjectMapper<ServerLogFileInfo> _csvToObjectMapperFileInfoMock;
        private CsvToObjectMapper<ServerLogFileInfo> _csvToObjectMapperFileInfo;
        private ILoggerFactory _loggerFactoryCsvReaderMock;

        private CsvToObjectMapper<ServerLogFactInfo> _csvToObjectMapperFactInfo;
        private CsvToObjectReader<ServerLogFileInfo> _readerLogInfo;
        private CsvToObjectReader<ServerLogFactInfo> _readerLogFactInfo;
        public CsvToObjectReaderTests()
        {
            _fileServiceMock = Substitute.For<IFileService>();
            _loggerFactoryCsvReaderMock = Substitute.For<ILoggerFactory>();
            _fileServiceReal = new FileService();
            _csvToObjectMapperFileInfoMock = Substitute.For<CsvToObjectMapper<ServerLogFileInfo>>();
        }

        [Fact(DisplayName = "PreValidation should fail if null values are passed in constructor")]
        [Trait("Unit Test","CsvToObjectReader object tests")]
        public void CheckPreValidations()
        {
            _readerLogInfo = new CsvToObjectReader<ServerLogFileInfo>(null,null,null,_loggerFactoryCsvReaderMock);
            bool preExtractValidation = _readerLogInfo.PreExtractValidation();
            preExtractValidation.Should().Be(false, " all the mandatory supplied parameters were not supplied");
            _readerLogInfo.ErrorsOccured.Count.Should().BeGreaterThan(0, "at least one error should be recorded");
            _readerLogInfo.ErrorsOccured.Any(errors => errors.ErrorCode == ErrorCodes.NullPath).Should()
                .Be(true, "we passed null parameters");
            _readerLogInfo.ErrorsOccured.Any(errors => errors.ErrorDescription == "Please supply the file content or provide the path to the file in constructor argument").Should()
                .Be(true, "we passed null parameters");
            _readerLogInfo.ErrorsOccured.Any(errors => errors.ErrorDescription == @"Constructor parameter '_mapper' cannot be null").Should()
                .Be(true, "we haven't passed mapper instance to constructor");
        }

        [Fact(DisplayName = "Should throw error if the input file doesn't exists")]
        [Trait("Unit Test", "CsvToObjectReader object tests")]
        public void InvalidPathShouldNotbeAccepted()
        {
            _readerLogInfo = new CsvToObjectReader<ServerLogFileInfo>($@"X:\InvalidFolder\InvalidFile.csv",new FileService() , null,_loggerFactoryCsvReaderMock);
            bool preExtractValidation = _readerLogInfo.PreExtractValidation();
            preExtractValidation.Should().Be(false, "Invalid Path was supplied");
            _readerLogInfo.ErrorsOccured.Count.Should().BeGreaterThan(0, "at least one error should be recorded");
            _readerLogInfo.ErrorsOccured.Any(errors => errors.ErrorDescription == @"X:\InvalidFolder\InvalidFile.csv  does not exists").Should()
                .Be(true, "we passed Invalid file path as constructor parameter");


            _readerLogInfo = new CsvToObjectReader<ServerLogFileInfo>($@"X:\InvalidFolder\InvalidFile.pdf", new FileService(), null,_loggerFactoryCsvReaderMock);
             preExtractValidation = _readerLogInfo.PreExtractValidation();
            _readerLogInfo.ErrorsOccured.Any(errors => errors.ErrorDescription.Contains(@"Invalid file extension")).Should()
                .Be(true, "we passed Invalid file extension");
        }

        [Fact(DisplayName = "Expected to Read 6 records in CSV file and transform to list of <ServerLogFileInfo>")]
        [Trait("Unit Test", "CsvToObjectReader object tests")]
        public void ConvertRecordsFromCsvTo_ServerLogFileInfo_List()
        {
                     var result = GetServerLogFileInfoList();
            result.Count().Should().Be(6, " csv file contains 6 records");
            (result[0].FileId + "," + result[0].FileName).Should()
                .Be("1,c:\\program files\\sql server\\master.mdf", "Record 1 should be as expected");

            (result[1].FileId + "," + result[5].FileName).Should()
                .Be("2,c:\\program files\\sql server\\customer-part-2012.mdf", "Record 2 should be as expected");

            
            (result[5].FileId + "," + result[5].FileName).Should()
                .Be("6,c:\\program files\\sql server\\customer-part-2012.mdf", "Record 2 should be as expected");

        }

        private List<ServerLogFileInfo> GetServerLogFileInfoList()
        {
            string path = TestHelpers.GetPathToTestDataFolder("InputFiles");
            _csvToObjectMapperFileInfo = new CsvToObjectMapper<ServerLogFileInfo>();
            _csvToObjectMapperFileInfo.AddMap((t) => t.FileId, "ID");
            _csvToObjectMapperFileInfo.AddMap(t => t.FileName, "Name");

            _readerLogInfo = new CsvToObjectReader<ServerLogFileInfo>($@"{path}\Files_6_rows_sample.csv", _fileServiceReal, _csvToObjectMapperFileInfo,_loggerFactoryCsvReaderMock);
            return _readerLogInfo.Read().ToList();
        }
        private List<ServerLogFactInfo> GetServerLogFactInfoList()
        {
            string path = TestHelpers.GetPathToTestDataFolder("InputFiles");
            _csvToObjectMapperFactInfo = new CsvToObjectMapper<ServerLogFactInfo>();
            _csvToObjectMapperFactInfo.AddMap(t => t.FileId, "FileID");
            _csvToObjectMapperFactInfo.AddMap(t => t.SizeInBytes, "SizeInBytes");
            _csvToObjectMapperFactInfo.AddMap(t => t.TimeStamp, "Timestamp");
            _readerLogFactInfo = new CsvToObjectReader<ServerLogFactInfo>(
                path + @"\FileStats_18_rows_sample_3_files.csv", new FileService(), _csvToObjectMapperFactInfo,_loggerFactoryCsvReaderMock);
            return _readerLogFactInfo.Read().ToList();
        }


        [Fact(DisplayName = "Expected to Read 18 records in CSV file and transform to list of <ServerLogFactInfo>")]
        [Trait("Unit Test", "CsvToObjectReader object tests")]
        public void ConvertRecordsFromCsvTo_ServerLogFactInfo_List()
        {
            
            var result = GetServerLogFactInfoList();


            result.Count().Should().Be(18, " csv file contains 18 records");
            (result[0].FileId + "," + result[0].SizeInBytes).Should()
                .Be("1,4245143", "Record 1 should be as expected");
            (result[17].FileId + "," + result[17].SizeInBytes).Should()
                .Be("6,1652318", "Last Record should be as expected");

        }


        [Fact(DisplayName = "Generate List of ServerLogFactGrowthInfo from given file and file stats <ServerLogFactInfo>")]
        [Trait("Unit Test", "CsvToObjectReader object tests")]
        public void GenerateFileGrowhFactStats()
        {
            var fileInfoList = GetServerLogFileInfoList();
            var factFileList = GetServerLogFactInfoList();
            ServerLogFactGrowthInfoGenerator<ServerLogFactGrowthInfo> slfg = new ServerLogFactGrowthInfoGenerator<ServerLogFactGrowthInfo>(fileInfoList.Cast<IServerLogFileInfo>().ToList(), factFileList.Cast<IServerLogFactInfo>().ToList());
            var sliced = slfg.GenerateSlicedList();
            sliced.Count().Should().Be(6, "Six files should be in the list");
            foreach (var grp in sliced)
            {
                grp.Count().Should().Be(2, "Each file should contain two entries");
            }
        }

        [Theory(DisplayName = "Verify file Growth Rate formula")]
        [Trait("Unit Test", "CsvToObjectReader object tests")]
        [MemberData(nameof(FileGrowthRateMultipleInputs))]
        public void VerifyFileGrowthRateFormula(double lastFileSize, double currentFileSize, DateTime lastTimeStamp, DateTime currentTimeStamp,double expectedGrowthInBytes)
        {
            var fileInfoList = Substitute.For<List<IServerLogFileInfo>>();
            var factFileList = Substitute.For<List<IServerLogFactInfo>>();
            ServerLogFactGrowthInfoGenerator<ServerLogFactGrowthInfo> slfg = new ServerLogFactGrowthInfoGenerator<ServerLogFactGrowthInfo>(fileInfoList, factFileList);
            var growthInBytes = slfg.CalculateLogFileGrowthPerHourInBytes(lastFileSize, currentFileSize, lastTimeStamp, currentTimeStamp);
            Math.Round(growthInBytes,1).Should().Be(expectedGrowthInBytes);
        }

        public static IEnumerable<object[]> FileGrowthRateMultipleInputs
            => new[]
            {
                new object[] {1000, 2000, DateTime.Parse("01-Jan-2018 12:00 AM"),DateTime.Parse("01-Jan-2018 1:00 AM"),1000 },
                new object[] { 4245143, 4276852, DateTime.Parse("2015-03-25 23:00:16.902"),DateTime.Parse("2015-03-25 23:55:45.787"), 34291.5 },
                new object[] { 4245143, 4245143, DateTime.Parse("2015-03-25 23:00:16.902"),DateTime.Parse("2015-03-25 23:00:16.902"), 0 },
                new object[] { 4245143, 4245143, DateTime.Parse("2015-03-25 23:00:16.902"), DateTime.Parse("2015-03-25 23:00:16.902"), 0 },
                new object[] { 8004366, 8055373, DateTime.Parse("2015-03-26 05:59:23.173"), DateTime.Parse("2015-03-26 07:00:10.239"), 50348.7 }
            };

        public void Dispose()
        {
        }
    }
}
