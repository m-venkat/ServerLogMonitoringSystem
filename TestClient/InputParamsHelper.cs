using System;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace ServerLogSizeMonitoring.Console
{
    public class InputParamsHelper
    {
       


        /// <summary>
        /// Function to display the help for command line Parameters
        /// </summary>
        public static void DisplayHelp()
        {

            try
            {
                XmlDocument displayDocument = new XmlDocument();
                displayDocument.Load(@".\CommandLineInfo.xml");
                var displayString = displayDocument.SelectSingleNode("commandlineinfo").InnerText;
                System.Console.Clear();
                System.Console.WriteLine(displayString);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"{ex.Message}\n{ex.StackTrace}");
                Environment.Exit(0);
            }

        }

        private static bool ValidateInputPath(string path, string folderOrFile)
        {
            if (folderOrFile == "File")
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    System.Console.WriteLine($"File path {path} is empty or does not exist.");
                    return false;
                }
            }
            else if (folderOrFile == "Folder")
            {
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                {
                    System.Console.WriteLine($"Directory path {path} is empty or does not exist.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get the program options from the user's input
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static InputParams ParseCommandLineParams(string[] args)
        {
            try
            {
                InputParams programInput = new InputParams();

                //setting the default values
                programInput.LogLevel = LogLevel.Information;
                programInput.FilePath = string.Empty;
                programInput.FactFilePath = string.Empty;
                programInput.OutputFolder = string.Empty;

                if (args.Length == 0)
                    return null;

                PropertyInfo propertyInfo = null;
                foreach (var argument in args)
                {
                    if (string.IsNullOrEmpty(argument?.Trim()))
                        break;

                    switch (argument.ToUpper().Trim())
                    {

                        case "-FILEPATH":
                            propertyInfo = (programInput.GetType()).GetProperty("FilePath");
                            break;
                        case "-FACTFILEPATH":
                            propertyInfo = (programInput.GetType()).GetProperty("FactFilePath");
                            break;
                        case "-OUTPUTDIR":
                            propertyInfo = (programInput.GetType()).GetProperty("OutputFolder");
                            break;
                        case "-LOGLEVEL":
                            propertyInfo = (programInput.GetType()).GetProperty("LogLevel");
                            break;
                        case "--HELP":
                        case "-H":
                        case "/?":
                            DisplayHelp();
                            break;

                        default:
                            if (propertyInfo != null)
                            {

                                if (propertyInfo.Name == "FilePath" || propertyInfo.Name == "FactFilePath" || propertyInfo.Name == "OutputFolder")
                                {
                                    propertyInfo.SetValue(programInput, argument);
                                    propertyInfo = null;
                                }
                                else if (propertyInfo.Name == "LogLevel")
                                {

                                    if (Enum.TryParse(typeof(LogLevel), argument, true, out var logLevel))
                                    {
                                        propertyInfo.SetValue(programInput, logLevel);
                                        propertyInfo = null;
                                    }

                                }

                            }
                            break;
                    }
                }

                bool filePathExists = ValidateInputPath(programInput.FilePath, "File");
                bool fileFactPathExists = ValidateInputPath(programInput.FactFilePath, "File");
                bool outputFolder = ValidateInputPath(programInput.OutputFolder, "Folder");
                if (!filePathExists || !fileFactPathExists || !outputFolder)
                {
                    Environment.Exit(0);
                }


                return programInput;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}