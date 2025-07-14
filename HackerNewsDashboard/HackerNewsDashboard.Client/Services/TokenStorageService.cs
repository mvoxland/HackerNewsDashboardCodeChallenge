using Blazored.LocalStorage;
using HackerNewsDashboard.Common.DTO;
using System.Text.Json;

namespace HackerNewsDashboard.Client.Services;

public class TokenStorageService : ITokenStorageService
{
    private readonly ILocalStorageService _storage;

    public TokenStorageService(ILocalStorageService storage)
    {
        _storage = storage;
    }

    public async Task SetTokenAsync(Token token)
    {
        await _storage.SetItemAsStringAsync("authToken", JsonSerializer.Serialize(token));
    }

    public async Task<Token?> GetTokenAsync()
    {
        var result = await _storage.GetItemAsStringAsync("authToken");

        if (result == null)
            return null;

        var resultObject = JsonSerializer.Deserialize<Token>(result);
        return resultObject;
    }

    public async Task RemoveTokenAsync() =>
        await _storage.RemoveItemAsync("authToken");
}
