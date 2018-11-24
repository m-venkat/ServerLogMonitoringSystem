
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace ServerLogMonitoringSystem.Tests
{
    public class TestHelpers
    {
        public static string GetPathToTestDataFolder(string inputFolder)
        {
            string startupPath = AppDomain.CurrentDomain.BaseDirectory;
            var pathItems = startupPath.Split(Path.DirectorySeparatorChar);
            var pos = pathItems.Reverse().ToList().FindIndex(x => string.Equals("bin", x));
            string projectPath = String.Join(Path.DirectorySeparatorChar.ToString(), pathItems.Take(pathItems.Length - pos - 1));
            return Path.Combine(projectPath, "TestDataFiles", inputFolder);
        }

    }
}
