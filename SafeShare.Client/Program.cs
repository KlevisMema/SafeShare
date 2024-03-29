using MudBlazor;
using SafeShare.Client;
using MudBlazor.Services;
using System.Text.Unicode;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using SafeShare.Client.Helpers;
using System.Text.Encodings.Web;
using SafeShare.ClientServices.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using SafeShare.ClientServices.UserManagment;
using SafeShare.ClientServices.GroupManagment;
using SafeShare.ClientServices.Authentication;
using SafeShare.ClientServices.ExpenseManagment;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;

    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 3000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.AddHttpClient("MyHttpClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7280/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddHttpMessageHandler<TokenExpiryHandler>();

builder.Services.AddSingleton<AppState>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<TokenExpiryHandler>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IClientService_UserManagment, ClientService_UserManagment>();
builder.Services.AddScoped<IClientService_GroupManagment, ClientService_GroupManagment>();
builder.Services.AddScoped<IClientService_ExpenseManagment, ClientService_ExpenseManagment>();
builder.Services.AddScoped<IClientAuthentication_TokenRefreshService, ClientAuthentication_TokenRefreshService>();

builder.Services.AddSingleton(HtmlEncoder.Create(allowedRanges: [ UnicodeRanges.BasicLatin,UnicodeRanges.CjkUnifiedIdeographs ]));

await builder.Build().RunAsync();
