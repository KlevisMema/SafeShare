using Serilog;
using SafeShare.ProxyApi.Helpers;
using Microsoft.AspNetCore.Builder;
using SafeShare.DataAccessLayer.Context;
using SafeShare.ProxyApi.Helpers.Middlewares;
using SafeShare.ProxyApi.Container.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InjectServices(builder.Configuration, builder.Host);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<API_Helper_ProxyForwardUserIpMiddleware>();

app.UseMiddleware<API_Helper_ProxyForwardAntiForgeryToken>();

app.UseCors(builder.Configuration.GetSection("Cors:Policy:Name").Value!);

app.UseMiddleware<API_Helper_JwtCookieToHeaderMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHubProxyService>("/notifications");

app.UseHsts();

app.UseSerilogRequestLogging();

app.Run();
