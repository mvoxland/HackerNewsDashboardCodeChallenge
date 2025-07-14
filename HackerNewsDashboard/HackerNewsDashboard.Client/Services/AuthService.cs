using HackerNewsDashboard.Client.Providers;
using HackerNewsDashboard.Common.DTO;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;

namespace HackerNewsDashboard.Client.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStorageService _tokenLocalStorage;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthService(
        HttpClient httpClient,
        ITokenStorageService localStorage,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _tokenLocalStorage = localStorage;
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<HttpResponseMessage?> Register(Register model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/register", model);

        if (response is null || !response.IsSuccessStatusCode)
            return response;

        var loginResult = await Login(new Common.DTO.Login() { Username = model.Username, Password = model.Password });

        return response;
    }

    public async Task<HttpResponseMessage?> Login(Login model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/login", model);

        if (response is null || !response.IsSuccessStatusCode)
            return response;

        var responseAsString = await response.Content.ReadAsStringAsync();

        var responseObject = JsonSerializer.Deserialize<Token>(responseAsString, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });

        if (responseObject is null)
            return null;

        await _tokenLocalStorage.SetTokenAsync(responseObject);

        var token = responseObject.AccessToken;

        ((JwtAuthStateProvider)_authenticationStateProvider).NotifyUserAuthentication(token);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return response;
    }

    public async Task<HttpResponseMessage?> RefreshToken()
    {
        var currentToken = await _tokenLocalStorage.GetTokenAsync();

        if (currentToken is null) return null;

        var response = await _httpClient.PostAsJsonAsync("api/refreshToken", currentToken);

        if (response is null || !response.IsSuccessStatusCode)
            return response;

        var responseAsString = await response.Content.ReadAsStringAsync();

        var responseObject = JsonSerializer.Deserialize<Token>(responseAsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (responseObject is null)
            return null;

        await _tokenLocalStorage.SetTokenAsync(responseObject);

        var token = responseObject.AccessToken;

        ((JwtAuthStateProvider)_authenticationStateProvider).NotifyUserAuthentication(token);

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return response;
    }

    public async Task Logout()
    {
        await _tokenLocalStorage.RemoveTokenAsync();

        ((JwtAuthStateProvider)_authenticationStateProvider).NotifyUserLogout();

        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}
