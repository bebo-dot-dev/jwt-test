using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace jwt_test.Services
{
    public abstract class HttpClientBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpClientBase> _logger;

        protected HttpClientBase(
            HttpClient httpClient,
            ILogger<HttpClientBase> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        
        protected async Task<TResponse> GetAsync<TResponse>(string path, string version)
        {
            var uri = new Uri(Path.Combine(_httpClient.BaseAddress.AbsoluteUri, path));
            SetVersionHeader(version);
            using var responseMessage = await _httpClient.GetAsync(uri);
            return await GetResponse<TResponse>(uri, "GET", responseMessage);
        }
        
        protected async Task<TResponse> PostAsync<TRequest, TResponse>(string path, string version, TRequest request)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var uri = new Uri(Path.Combine(_httpClient.BaseAddress.AbsoluteUri, path));
            SetVersionHeader(version);
            using var responseMessage = await _httpClient.PostAsync(uri, requestContent);
            return await GetResponse<TResponse>(uri, "POST", responseMessage, requestContent);
        }
        
        protected async Task<TResponse> PutAsync<TRequest, TResponse>(string path, string version, TRequest request)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var uri = new Uri(Path.Combine(_httpClient.BaseAddress.AbsoluteUri, path));
            SetVersionHeader(version);
            using var responseMessage = await _httpClient.PutAsync(uri, requestContent);
            return await GetResponse<TResponse>(uri, "PUT", responseMessage, requestContent);
        }
        
        protected async Task<TResponse> DeleteAsync<TResponse>(string path, string version)
        {
            var uri = new Uri(Path.Combine(_httpClient.BaseAddress.AbsoluteUri, path));
            SetVersionHeader(version);
            using var responseMessage = await _httpClient.DeleteAsync(uri);
            return await GetResponse<TResponse>(uri, "DELETE", responseMessage);
        }

        private void SetVersionHeader(string version)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-version", version);
        }

        private async Task<TResponse> GetResponse<TResponse>(
            Uri uri,
            string httpVerb, 
            HttpResponseMessage responseMessage, 
            HttpContent? requestContent = null)
        {
            var response = await responseMessage.Content.ReadAsStringAsync();

            if (!responseMessage.IsSuccessStatusCode)
            {
                if (requestContent != default)
                {
                    var request = await requestContent.ReadAsStringAsync();
                    _logger.LogWarning(
                        "The HTTP {httpVerb} request to {uri} returned unsuccessful" +
                        " HTTP status code {statusCodeInt} {statusCode}.\r\nRequest payload:\r\n" +
                        "{requestPayload}\r\n" +
                        "Response payload:\r\n{responseBody}", 
                        httpVerb, uri.AbsoluteUri, (int)responseMessage.StatusCode, responseMessage.StatusCode, request, response);   
                }
                else
                {
                    _logger.LogWarning(
                        "The HTTP {httpVerb} request to {uri} returned unsuccessful" +
                        " HTTP status code {statusCodeInt} {statusCode}.\r\nResponse payload:\r\n" +
                        "{responseBody}", 
                        httpVerb, uri.AbsoluteUri, (int)responseMessage.StatusCode, responseMessage.StatusCode, response);    
                }
            }
            
            return JsonSerializer.Deserialize<TResponse>(response);
        }
    }
}

