using Serilog;
using System.Text;
using System.Reflection;
using AspNetCoreRateLimit;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Security.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SafeShare.Utilities.SafeShareApi.ConfigurationSettings;
using SafeShare.ProxyApi.Container.Services;
using SafeShare.ProxyApi.Container.Interfaces;
using SafeShare.ProxyApi.Helper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("ProxyHttpClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7046/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var jwtSetting = builder.Configuration.GetSection(Util_JwtSettings.SectionName);

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

})
.AddJwtBearer("ConfirmLogin", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSetting.GetSection("Issuer").Value,
        ValidAudience = jwtSetting.GetSection("Audience").Value,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.GetSection("KeyConfrimLogin").Value!)),
    };
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

    options.AddSecurityDefinition("ConfirmLogin", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer' [Space] and then your token in the input field below.",
        Type = SecuritySchemeType.Http,
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
                            Id = "ConfirmLogin"
                        },
                        Scheme = "0auth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });

    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Name = "X-Api-Key",
        Description = "API Key authentication",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" },
                            Name = "X-Api-Key",
                            In = ParameterLocation.Header
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

    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    //options.IncludeXmlComments(xmlPath);
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(builder.Configuration.GetSection("Cors:Policy:Name").Value!, builder =>
    {
        builder.WithOrigins("https://localhost:7027", "https://localhost:7038", "http://localhost:5026", "https://127.0.0.1")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules =
    [
        new RateLimitRule
        {
            Endpoint = "*",
            Limit = 100,
            PeriodTimespan = TimeSpan.FromMinutes(1)
        }
    ];
});

builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();


builder.Services.AddScoped<IProxyAuthentication, ProxyAuthentication>();

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

app.UseSerilogRequestLogging();

app.Run();
