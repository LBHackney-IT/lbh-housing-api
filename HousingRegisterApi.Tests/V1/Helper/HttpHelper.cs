using Newtonsoft.Json;
using System.Net.Http;

namespace HousingRegisterApi.Tests.V1.Helper
{
    public static class HttpHelper
    {
        public static T GetResponse<T>(this HttpResponseMessage response){

            string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonConvert.DeserializeObject<T>(responseBody);
        }

    }
}
