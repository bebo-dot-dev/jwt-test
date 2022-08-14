using System.Net.Http;
using System.Threading.Tasks;
using jwt_test.Contract;
using Microsoft.Extensions.Logging;

namespace jwt_test.Services
{
    /// <summary>
    /// A sample http service client
    /// </summary>
    public class ServiceClient : HttpClientBase, IServiceClient
    {
        public ServiceClient(HttpClient httpClient, ILogger<HttpClientBase> logger)
            : base(httpClient, logger) { }

        public async Task<ResponseModel> DoGet(RequestModel request)
        {
            var path = $"api/test?someValue={request.SomeValue}&someString={request.SomeString}";
            return await GetAsync<ResponseModel>(path, "1.0");
        }

        public async Task<ResponseModel> DoPost(RequestModel request)
        {
            return await PostAsync<RequestModel, ResponseModel>("api/test", "1.0", request);
        }
    }
}