using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HousingRegisterApi.V1.Boundary.Request;
using HousingRegisterApi.V1.Boundary.Response;

namespace HousingRegisterApi.V1.UseCase.Interfaces
{
    public interface ICreateEvidenceRequestUseCase
    {
        Task<List<EvidenceRequestResponse>> ExecuteAsync(Guid id, CreateEvidenceRequestBase request);
    }
}
