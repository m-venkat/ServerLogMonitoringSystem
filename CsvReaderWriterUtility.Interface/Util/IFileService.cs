using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CsvReadWriteUtility.Utils
{
    public interface IFileService
    {
        string PathCombine(string directoryName, string fileName);
        bool DirectoryExists(string path);
        DirectoryInfo CreateDirectory(string path);
        void WriteAllText(string path, string text);
        void WriteAllLines(string path, string[] text);
        bool FileExists(string path);
        string[] ReadAllLines(string path);
        string[] ReadAllText(string path);
        string PathGetExtension(string pathToCsv);
        void DeleteFile(string s);
    }
}
