using Serilog;
using System.Text;
using System.Configuration;
using Humanizer.Configuration;
using Microsoft.OpenApi.Models;
using API_Client.BLL.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using API_Client.BLL.Implementation;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SafeShare.DataAccessLayer.Context;
using Microsoft.Extensions.Configuration;
using SafeShare.Utilities.SafeShareApiKey.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SafeShare.DataAccessLayer.Models.SafeShareApiKey;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApiClientDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));

// do not remove
//if (Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING") == null)
//{
//    builder.Services.AddDbContext<ApiClientDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));
//}
//else
//{
//    builder.Services.AddDbContext<ApiClientDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING")));
//}


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
    using var serviceScope = app.Services.CreateScope();

    var _apiContext = serviceScope.ServiceProvider.GetService<ApiClientDbContext>();

    _apiContext?.Database.EnsureCreated();

    app.UseSwagger();
    app.UseSwaggerUI();

    if (!_apiContext.Clients.Any())
    {
        var newClient = new ApiClient
        {
            AccessFailedCount = 0,
            CompanyName = "test",
            ContactPerson = "test",
            Description = "test",
            Email = "test@gmail.com",
            IsActive = true,
            NormalizedEmail = "TEST",
            UserName = "test.test",
            NormalizedUserName = "TEST.TEST",
            PhoneNumber = "45435344543",
            PhoneNumberConfirmed = true,
            RegisteredOn = DateTime.UtcNow,
            TwoFactorEnabled = false,
            Website = "https://test",
            SiteYouDevelopingUrl = "https://test2",
            EmailConfirmed = true,
            LockoutEnabled = false,
            LockoutEnd = null,
            ConcurrencyStamp = null,
            SecurityStamp = null,
            Id = "5a0aa964-31f1-447a-bd2c-00f6fe739502",
        };

        _apiContext.Clients.Add(newClient);

        var apiKey = new ApiKey
        {
            ApiClientId = newClient.Id,
            ApiKeyId = Guid.Parse("afb98543-2c4c-487e-870a-efb0c4b59ca9"),
            CreatedOn = DateTime.UtcNow,
            Environment = SafeShare.Utilities.SafeShareApi.Enums.WorkEnvironment.Testing,
            ExpiresOn = DateTime.UtcNow.AddDays(7),
            IsActive = true,
            ApiClient = newClient,
            KeyHash = "1f467cdafbdf1532e9734dc4c45234958331267e51964892363856817d972ba4"
        };

        _apiContext.ApiKeys.Add(apiKey);

        _apiContext.SaveChanges();
    }
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseSerilogRequestLogging();

app.Run();
