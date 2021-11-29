using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Domain.Report;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase.Interfaces
{
    public interface IGetInternalReportUseCase
    {
        Task<ExportFile> Execute(InternalReportRequest request);
    }
}
