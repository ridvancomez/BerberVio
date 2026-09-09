using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BerberVio.Services;

public class ApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]!);
        AddAuthHeader(client);
        return client;
    }

    private void AddAuthHeader(HttpClient client)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["jwt"];
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<HttpResponseMessage> GetAsync(string requestUri)
    {
        var client = CreateClient();
        return await client.GetAsync(requestUri);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string requestUri, T data)
    {
        var client = CreateClient();
        return await client.PostAsJsonAsync(requestUri, data);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string requestUri, T data)
    {
        var client = CreateClient();
        return await client.PutAsJsonAsync(requestUri, data);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
    {
        var client = CreateClient();
        return await client.DeleteAsync(requestUri);
    }
}
