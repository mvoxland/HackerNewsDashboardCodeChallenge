using HackerNewsDashboard.Client.Services;

namespace HackerNewsDashboard.Client.Handlers;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly ITokenStorageService _tokenStorageService;

    public AuthMessageHandler(ITokenStorageService tokenStorageService)
    {
        _tokenStorageService = tokenStorageService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenStorageService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token?.AccessToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
