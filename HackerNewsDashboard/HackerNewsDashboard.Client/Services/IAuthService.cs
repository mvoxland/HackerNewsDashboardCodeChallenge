using HackerNewsDashboard.Common.DTO;

namespace HackerNewsDashboard.Client.Services;

public interface IAuthService
{
    Task<HttpResponseMessage?> Register(Register model);

    Task<HttpResponseMessage?> Login(Login model);

    Task<HttpResponseMessage?> RefreshToken();

    Task Logout();
}
