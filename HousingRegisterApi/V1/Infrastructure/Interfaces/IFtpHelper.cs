using System.Threading.Tasks;

namespace HousingRegisterApi.V1.Infrastructure
{
    public interface IFtpHelper
    {
        bool UploadDataToFtp(byte[] data, string fileName);
    }
}
