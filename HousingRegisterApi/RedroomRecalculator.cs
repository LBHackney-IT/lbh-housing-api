using Amazon.Lambda.AspNetCoreServer;
using Microsoft.AspNetCore.Hosting;

namespace HousingRegisterApi
{
    public class BedroomRecalculator
    {
        protected override void Recalculate()
        {
            throw new Exception('this is happening');
        }
    }
}
