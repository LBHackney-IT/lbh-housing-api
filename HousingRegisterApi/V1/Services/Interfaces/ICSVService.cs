using HousingRegisterApi.V1.Domain.Report;
using System;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Services
{
    public interface ICsvService
    {
        /// <summary>
        /// Generate CSV using default configuration
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        Task<byte[]> Generate(Array source);

        /// <summary>
        /// Generate CSV with custom configuration
        /// </summary>
        /// <param name="source"></param>
        /// <param name="csvParameters"></param>
        /// <returns></returns>
        Task<byte[]> Generate(Array source, CsvParameters csvParameters);
    }
}
