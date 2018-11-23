using System;
using System.ComponentModel;
using CsvReadWriteUtility.Exceptions;
using CsvReadWriteUtility.Parser;
using FluentAssertions;
using ServerLogGrowthTracker.FileInfo;
using Xunit;

//https://fluentassertions.com/documentation/ [Quick reference]
namespace ServerLogMonitoringSystem.Tests
{


    [DisplayName("CsvToObjectMapper Tests")]
    public class CsvToObjectMapperTests : IDisposable
    {
        public class ClassWithPublicField
        {
            public string PublicStringField = string.Empty;
            public string PublicStringProperty { get; set; }
        }
        private CsvToObjectMapper<ServerLogFactGrowthInfo> _csvToObjectMapper;
        public CsvToObjectMapperTests()
        {
            
        }

        [Fact(DisplayName = "Mandatory parameter validation for ObjectToCsvMapper.Add")]
        public void CsvToObjectMapper_AddMap_MandatoryParameterValueTests()
        {
            _csvToObjectMapper = new CsvToObjectMapper<ServerLogFactGrowthInfo>();
            var ex = Assert.Throws<CsvReadWriteException>(() => _csvToObjectMapper.AddMap(t => t.FileId, null));
            ex.Message.Should().Be("csvColumnName cannot be null or empty","csvColumnName is mandatory attribute for mapping");
            
        }

        [Fact(DisplayName = "Ability to add item to ObjectToCsvMapper")]
        public void Able_to_Add_ObjectToCsv_Mapping_To_Mapper()
        {
            _csvToObjectMapper = new CsvToObjectMapper<ServerLogFactGrowthInfo>();
            _csvToObjectMapper.AddMap(t => t.FileId, "1.csv");
            _csvToObjectMapper.ObjectToCsvMapping.Count.Should().Be(1, " We have just added one item to map");
        }

        [Fact(DisplayName = "Consider the last added map to the ObjectToCsvMapper if duplicate entries added")]
        public void When_Duplicate_Mapping_Added_Consider_The_Last_Added_Value()
        {
            _csvToObjectMapper = new CsvToObjectMapper<ServerLogFactGrowthInfo>();
            _csvToObjectMapper.AddMap(t => t.FileId, "1.csv");
            _csvToObjectMapper.AddMap(t => t.FileName, "2.csv");
            _csvToObjectMapper.AddMap(t => t.FileId, "3.csv");
            _csvToObjectMapper.ObjectToCsvMapping.Count.Should().Be(2, " If we add same item twice, it should only retain the last value");
        }

        [Fact(DisplayName = "Dont allow empty space string as csv column name")]
        public void DontAllowEmptySpaceStringAsCsvColumName()
        {
            _csvToObjectMapper = new CsvToObjectMapper<ServerLogFactGrowthInfo>();
            var ex = Assert.Throws<CsvReadWriteException>(() => _csvToObjectMapper.AddMap(t => t.FileId, "      "));
            ex.Message.Should().Be("csvColumnName cannot be null or empty", "It should not allow to add empty spaces as csv column");
        }

        [Fact(DisplayName = "Should not allow same csv column to mapped to two different properties")]
        public void CannotAddSameCsvColumnToTwoDifferentProperties()
        {
            _csvToObjectMapper = new CsvToObjectMapper<ServerLogFactGrowthInfo>();
            _csvToObjectMapper.AddMap(t => t.FileId, "FileId");
            var ex = Assert.Throws<CsvReadWriteException>(() => _csvToObjectMapper.AddMap(t => t.TimeStamp, "FileId"));
            ex.Should().BeOfType<CsvReadWriteException>()
                .Which.Message.Should().Be($@"Csv column name 'FileId' already present in the mapping");
          }

        [Fact(DisplayName = "Allow mapping  for readable public property")]
        public void MappingToPublicPropertiesAllowed()
        {
            CsvToObjectMapper<ClassWithPublicField> publicField = new CsvToObjectMapper<ClassWithPublicField>();
            publicField.AddMap(t => t.PublicStringField, "FileId");
            publicField.ObjectToCsvMapping.Count.Should().Be(1, " public field should be allowed to map to csv");
        }

        public void Dispose()
        {
            _csvToObjectMapper = null;
        }
    }
}
