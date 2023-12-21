using Serilog;
using System.Text;
using System.Configuration;
using Humanizer.Configuration;
using Microsoft.OpenApi.Models;
using API_Client.BLL.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using API_Client.BLL.Implementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SafeShare.DataAccessLayer.Context;
using Microsoft.Extensions.Configuration;
using SafeShare.Utilities.SafeShareApiKey.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SafeShare.DataAccessLayer.Models.SafeShareApiKey;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApiClientDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING")));

builder.Services.AddIdentity<ApiClient, IdentityRole>
(
    options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 6;
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
    }
)
.AddEntityFrameworkStores<ApiClientDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

var jwtSetting = builder.Configuration.GetSection(JwtSettings.SectionName);

var defaultTokanValidationParameters = new TokenValidationParameters
{
    ValidateAudience = true,
    ValidateIssuer = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    RequireExpirationTime = true,
    RequireSignedTokens = true,
    ValidIssuer = jwtSetting.GetSection("Issuer").Value,
    ValidAudience = jwtSetting.GetSection("Audience").Value,
    ClockSkew = TimeSpan.Zero,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.GetSection("Key").Value!)),
};

builder.Services.AddSingleton(defaultTokanValidationParameters);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})
.AddJwtBearer("Default", options =>
{
    options.TokenValidationParameters = defaultTokanValidationParameters;

});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Default", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer' [Space] and then your token in the input field below.",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        BearerFormat = "Jwt"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Default"
                },
                Scheme = "0auth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    options.SwaggerDoc(builder.Configuration.GetSection("Swagger:Doc:Version").Value, new OpenApiInfo
    {
        Version = builder.Configuration.GetSection("Swagger:Doc:Version").Value,
        Title = builder.Configuration.GetSection("Swagger:Doc:Tittle").Value,
        License = new OpenApiLicense
        {
            Name = builder.Configuration.GetSection("Swagger:Doc:Licence:Name").Value,
            Url = new Uri(builder.Configuration.GetSection("Swagger:Doc:Licence:Url-Linkedin").Value!)
        }
    });
});

builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
builder.Services.AddScoped<IApiClientService, ApiClientService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

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
