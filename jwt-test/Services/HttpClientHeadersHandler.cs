using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace jwt_test.Services
{
    /// <summary>
    /// The HttpClientHeadersHandler responsible for assigning authorization bearer token and other general common headers
    /// to requests
    /// </summary>
    public class HttpClientHeadersHandler: DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpClientHeadersHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var bearerToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await base.SendAsync(request, cancellationToken);
        }
        
    }
}