using System;
using System.Collections.Generic;
using System.Text;

namespace CsvReadWriteUtility.Util
{
    public interface IFileService
    {
        bool FileExists(string path);
        string[] ReadAllLines(string path);
        string[] ReadAllText(string path);
        string PathGetExtension(string pathToCsv);
    }
}
