using Serilog;
using SafeShare.ProxyApi.Helper;
using Microsoft.AspNetCore.Builder;
using SafeShare.DataAccessLayer.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InjectServices(builder.Configuration, builder.Host);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<API_Helper_JwtCookieToHeaderMiddleware>();

app.UseCors(builder.Configuration.GetSection("Cors:Policy:Name").Value!);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseHsts();

app.UseMiddleware<API_Helper_ProxyForwardedHeadersMiddleware>();

app.UseSerilogRequestLogging();

app.Run();
