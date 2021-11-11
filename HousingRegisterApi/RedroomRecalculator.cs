using HousingRegisterApi.V1.UseCase.Interfaces;
using System;

namespace HousingRegisterApi
{
    public class BedroomRecalculator
    {

        private readonly IRecalculateBedroomsUseCase _recalculateBedroomsUseCase;

        public BedroomRecalculator(IRecalculateBedroomsUseCase recalculateBedroomsUseCase)
        {
            _recalculateBedroomsUseCase = recalculateBedroomsUseCase;
        }

        public void Recalculate()
        {
            //throw new Exception("this is happening");           
            _recalculateBedroomsUseCase.Execute();
        }
    }
}
