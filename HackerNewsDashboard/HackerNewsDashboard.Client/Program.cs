using Blazored.LocalStorage;
using HackerNewsDashboard.Client.Handlers;
using HackerNewsDashboard.Client.Providers;
using HackerNewsDashboard.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthMessageHandler>();
builder.Services.AddHttpClient("HackerNewsDashboardAuthorizedClient", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
}).AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("HackerNewsDashboardAuthorizedClient");
});

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
