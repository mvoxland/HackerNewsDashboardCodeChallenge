using HackerNewsDashboard.Client.Providers;
using HackerNewsDashboard.Client.Services;
using HackerNewsDashboard.Common.DTO;
using System.Net.Http;
using System.Text.Json;

namespace HackerNewsDashboard.Client.Handlers;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly ITokenStorageService _tokenStorageService;
    private readonly IAuthService _authService;

    public AuthMessageHandler(ITokenStorageService tokenStorageService,
        IAuthService authService)
    {
        _tokenStorageService = tokenStorageService;
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenStorageService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token?.AccessToken))
        {
            var expirationString = JwtAuthStateProvider.ParseClaimsFromJwt(token.AccessToken)?.FirstOrDefault(c => c.Type == "exp")?.Value;
            if (expirationString is not null && DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationString)).UtcDateTime <= DateTime.UtcNow)
            {
                var response = await _authService.RefreshToken();
                if (response is not null && response.IsSuccessStatusCode)
                {
                    var responseAsString = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<Token>(responseAsString);
                    if (responseObject is not null && !string.IsNullOrEmpty(responseObject.AccessToken) && !string.IsNullOrEmpty(responseObject.RefreshToken))
                    {
                        await _tokenStorageService.SetTokenAsync(responseObject);
                    }
                    else
                    {
                        await _tokenStorageService.RemoveTokenAsync();
                    }
                }
                else
                {
                    await _tokenStorageService.RemoveTokenAsync();
                }
            }
        }

        token = await _tokenStorageService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token?.AccessToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
