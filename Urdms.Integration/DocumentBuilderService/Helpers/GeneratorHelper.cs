using System;
using System.Configuration;
using System.IO;

namespace Urdms.DocumentBuilderService.Helpers
{
    public interface IGeneratorHelper
    {
        /// <summary>
        /// Creates the file path of the file to be generated.
        /// </summary>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns></returns>
        string CreateFilePath(string fileExtension);
    }

    public class GeneratorHelper : IGeneratorHelper
    {

        /// <summary>
        /// Creates the file path of the file to be generated.
        /// </summary>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns></returns>
        public string CreateFilePath(string fileExtension)
        {
            var tempStore = GetTmpDirectory();
            var fileName = BuildFileName(fileExtension);

            return Path.Combine(tempStore, fileName);
        }
        
        /// <summary>
        /// Gets the TMP directory.
        /// </summary>
        /// <returns></returns>
        private static string GetTmpDirectory()
        {
            var tmp = ConfigurationManager.AppSettings["TempStore"];

            if (string.IsNullOrWhiteSpace(tmp))
            {
                tmp = AppDomain.CurrentDomain.BaseDirectory;
            }

            return tmp;
        }

        /// <summary>
        /// Builds the name of the file.
        /// </summary>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns></returns>
        private static string BuildFileName(string fileExtension)
        {
            return string.Format("Research_Data_Management_Plan.{0}", fileExtension);
        }
    }
}
