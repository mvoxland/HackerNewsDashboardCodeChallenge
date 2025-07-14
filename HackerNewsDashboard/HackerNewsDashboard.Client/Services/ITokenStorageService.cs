using HackerNewsDashboard.Common.DTO;

namespace HackerNewsDashboard.Client.Services;

public interface ITokenStorageService
{
    Task SetTokenAsync(Token token);
    Task<Token?> GetTokenAsync();
    Task RemoveTokenAsync();
}
