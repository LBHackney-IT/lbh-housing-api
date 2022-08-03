using HousingRegisterApi.V1.Domain.Report;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Gateways
{
    public interface IFileGateway
    {
        /// <summary>
        /// Adds or updates a new or existing file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="parentFolderName"></param>
        /// <returns></returns>
        Task SaveFile(ExportFile file, string parentFolderName = "");

        /// <summary>
        /// Returns a file object for the specified file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="parentFolderName"></param>
        /// <returns></returns>
        Task<ExportFile> GetFile(string fileName, string parentFolderName = "");

        /// <summary>
        /// Returns a list of files that have been generated
        /// </summary>
        /// <param name="parentFolderName"></param>
        /// <param name="numberOfMonthsToReturn"></param>
        /// <returns></returns>
        List<ExportFileItem> ListFiles(string parentFolderName = "", int numberOfMonthsToReturn = 3);

        /// <summary>
        /// Gets the current attributes for a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="parentFolderName"></param>
        /// <returns></returns>
        Task<Dictionary<string, string>> GetAttributes(string fileName, string parentFolderName = "");

        /// <summary>
        /// Appends or updates a file's metadata
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="attributes"></param>
        /// <param name="parentFolderName"></param>
        /// <returns></returns>
        Task UpdateAttributes(string fileName, Dictionary<string, string> attributes, string parentFolderName = "");
    }
}
