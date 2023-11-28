using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using Dashboard.Services;
using Newtonsoft.Json.Linq;

namespace WebApi.Services;

public interface IHttpService
{
    Task<string> Get(string uri);
    Task<string> Post(string uri, object value);
    Task<string> Put(string uri, object value);
    Task<string> Delete(string uri);
}

public class HttpService : IHttpService
{
    private IHttpClientFactory _clientFactory;
    private NavigationManager _navigationManager;
    private ILocalStorageService _localStorageService;

    public HttpService(
        IHttpClientFactory clientFactory,
        NavigationManager navigationManager,
        ILocalStorageService localStorageService
    )
    {
        _clientFactory = clientFactory;
        _navigationManager = navigationManager;
        _localStorageService = localStorageService;
    }

    public async Task<string> Get(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        return await sendRequest(request);
    }

    public async Task<string> Post(string uri, object value)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
        return await sendRequest(request);
    }

    public async Task<string> Put(string uri, object value)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, uri);
        request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
        return await sendRequest(request);
    }

    public async Task<string> Delete(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, uri);
        return await sendRequest(request);
    }

    // helper methods

    private async Task<string> sendRequest(HttpRequestMessage request)
    {
        // add jwt auth header if user is logged in and request is to the api url
        var jwtToken = "";
        try
        {
            jwtToken = await _localStorageService.GetItem<string>("jwtToken");
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e);
        }
        var isApiUrl = !request.RequestUri.IsAbsoluteUri;
        if (jwtToken != null && isApiUrl)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var _httpClient = _clientFactory.CreateClient("LocalApi");
        using var response = await _httpClient.SendAsync(request);

        // auto logout on 401 response
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateTo("/Logout");
            return default;
        }

        // throw exception on error response
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception(error);
        }

        return await response.Content.ReadAsStringAsync();
    }
}