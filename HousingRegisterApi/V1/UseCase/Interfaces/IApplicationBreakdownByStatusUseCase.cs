using System.Collections.Generic;
using System.Threading.Tasks;

namespace HousingRegisterApi.V1.UseCase.Interfaces
{
    public interface IApplicationBreakdownByStatusUseCase
    {
        Task<Dictionary<string, long>> Execute();
    }
}
