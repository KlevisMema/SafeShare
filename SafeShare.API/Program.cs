using Serilog;
using SafeShare.API.Helpers;
using SafeShare.API.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Antiforgery;
using SafeShare.API.Helpers.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InjectServices(builder.Configuration, builder.Host);

var app = builder.Build();

await API_Helper_AfterAppBuild.Extension(app, builder.Configuration);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors(builder.Configuration.GetSection("Cors:Policy:Name").Value!);

//app.UseAntiforgery();

app.UseMiddleware<API_HELPER_ApiKeyMiddleware>(); 

app.UseMiddleware<API_Helper_ForgeryToken>();

app.MapControllers();

app.UseHsts();

app.UseSerilogRequestLogging();

app.Run();