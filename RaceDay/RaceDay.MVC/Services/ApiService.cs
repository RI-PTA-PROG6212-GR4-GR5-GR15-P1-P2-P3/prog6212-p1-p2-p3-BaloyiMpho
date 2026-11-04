using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RaceDay.MVC.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContext;

    public ApiService(IHttpClientFactory factory, IHttpContextAccessor httpContext)
    {
        _http = factory.CreateClient("RaceDayApi");
        _httpContext = httpContext;
    }

    private void AttachToken()
    {
        var token = _httpContext.HttpContext?.Session.GetString("JWToken");
        _http.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        AttachToken();
        var response = await _http.GetAsync(url);
        if (!response.IsSuccessStatusCode) return default;
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<(bool Success, string? Error)> PostAsync<T>(string url, T data)
    {
        AttachToken();
        var response = await _http.PostAsJsonAsync(url, data);
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await response.Content.ReadAsStringAsync());
    }

    public async Task<(bool Success, string? Error)> PostFormAsync(string url, MultipartFormDataContent content)
    {
        AttachToken();
        var response = await _http.PostAsync(url, content);
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await response.Content.ReadAsStringAsync());
    }

    public async Task<(bool Success, string? Error)> PutAsync<T>(string url, T data)
    {
        AttachToken();
        var response = await _http.PutAsJsonAsync(url, data);
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await response.Content.ReadAsStringAsync());
    }

    public async Task<(bool Success, string? Error)> PutEmptyAsync(string url)
    {
        AttachToken();
        var response = await _http.PutAsync(url, null);
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await response.Content.ReadAsStringAsync());
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(string url)
    {
        AttachToken();
        var response = await _http.DeleteAsync(url);
        if (response.IsSuccessStatusCode) return (true, null);
        return (false, await response.Content.ReadAsStringAsync());
    }

    public async Task<(T? Data, string? Error)> PostAndReadAsync<T>(string url, object data)
    {
        AttachToken();
        var response = await _http.PostAsJsonAsync(url, data);
        if (!response.IsSuccessStatusCode) return (default, await response.Content.ReadAsStringAsync());
        return (await response.Content.ReadFromJsonAsync<T>(), null);
    }
}
