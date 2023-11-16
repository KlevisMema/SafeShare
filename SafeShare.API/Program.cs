using Serilog;
using SafeShare.API.Helpers;
using SafeShare.API.Startup;
using Microsoft.Extensions.Configuration;

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

app.UseCors(builder.Configuration.GetSection("Cors:Policy:Name").Value!);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();

app.UseHsts();

app.UseSerilogRequestLogging();

app.Run();