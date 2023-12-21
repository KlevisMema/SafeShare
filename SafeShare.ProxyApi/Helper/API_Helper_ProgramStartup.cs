using Serilog;
using System.Text;
using AspNetCoreRateLimit;
using Microsoft.OpenApi.Models;
using System.Security.Authentication;
using Microsoft.IdentityModel.Tokens;
using SafeShare.ProxyApi.Container.Services;
using SafeShare.ProxyApi.Container.Interfaces;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SafeShare.Utilities.SafeShareApi.ConfigurationSettings;

namespace SafeShare.ProxyApi.Helper;

public static class API_Helper_ProgramStartup
{
    public static IServiceCollection InjectServices
    (
       this IServiceCollection Services,
       IConfiguration Configuration,
       IHostBuilder Host
    )
    {
        const string ClientName = "ProxyHttpClient";
        const string ApiBaseRoute = "https://localhost:7046/";

        Services.AddControllers();
        Services.AddEndpointsApiExplorer();
        Services.AddSwaggerGen();
        Services.AddMemoryCache();
        Services.AddHttpContextAccessor();

        Services.AddHttpClient(ClientName, client =>
        {
            client.BaseAddress = new Uri(ApiBaseRoute);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

        var jwtSetting = Configuration.GetSection(Util_JwtSettings.SectionName);

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
        Services.AddSingleton(defaultTokanValidationParameters);
        Services.AddAuthentication(options =>
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
        Services.AddSwaggerGen(options =>
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

            options.SwaggerDoc(Configuration.GetSection("Swagger:Doc:Version").Value, new OpenApiInfo
            {
                Version = Configuration.GetSection("Swagger:Doc:Version").Value,
                Title = Configuration.GetSection("Swagger:Doc:Tittle").Value,
                License = new OpenApiLicense
                {
                    Name = Configuration.GetSection("Swagger:Doc:Licence:Name").Value,
                    Url = new Uri(Configuration.GetSection("Swagger:Doc:Licence:Url-Linkedin").Value!)
                }
            });

            //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            //options.IncludeXmlComments(xmlPath);
        });

        Services.Configure<KestrelServerOptions>(options =>
        {
            options.ConfigureHttpsDefaults(httpsOptions =>
            {
                httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
            });
        });

        Services.AddCors(options =>
        {
            options.AddPolicy(Configuration.GetSection("Cors:Policy:Name").Value!, builder =>
            {
                builder.WithOrigins("https://localhost:7027", "https://localhost:7038", "http://localhost:5026", "https://127.0.0.1")
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
            });
        });

        Services.Configure<IpRateLimitOptions>(options =>
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

        Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        Services.AddScoped<IProxyAuthentication, ProxyAuthentication>();

        return Services;
    }

}