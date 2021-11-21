using HousingRegisterApi.V1.Domain.FileExport;
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
        /// <returns></returns>
        Task SaveFile(ExportFile file);

        /// <summary>
        /// Returns a file object for the specified file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<ExportFile> GetFile(string fileName);

        /// <summary>
        /// Appends or updates a file's metadata
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        Task UpdateMetadata(string fileName, Dictionary<string, string> metadata);
    }
}
