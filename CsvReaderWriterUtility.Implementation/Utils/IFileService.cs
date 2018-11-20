using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CsvReadWriteUtility.Util
{
    public class FileService :IFileService
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public string[] ReadAllText(string path)
        {
            return File.ReadAllLines(path);
        }

        public string PathGetExtension(string path)
        {
            return Path.GetExtension(path);
        }
    }
}
