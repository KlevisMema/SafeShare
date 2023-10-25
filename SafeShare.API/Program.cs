using Serilog;
using SafeShare.API.Helpers;
using SafeShare.API.Startup;

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

app.MapControllers();

app.UseSerilogRequestLogging();

app.Run();