using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CsvReadWriteUtility.Utils
{
    public class FileService :IFileService
    {
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public string PathCombine(string directoryName, string fileName)
        {
            return Path.Combine(directoryName, fileName);
        }
        public DirectoryInfo CreateDirectory(string path)
        {
           return Directory.CreateDirectory(path);
        }

        public void WriteAllText(string path,string text)
        {
            File.WriteAllText(path, text);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }
        public void WriteAllLines(string path,string[] text)
        {
            File.WriteAllLines(path, text);
        }

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
