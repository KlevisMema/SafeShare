/*
    Entry point for registering all services in Safe Share application.
    This class contains extension methods to inject services into the service collection.

    The ProgramExtension class provides methods to configure services such as adding databases,
    AutoMapper, Swagger, custom services, and CORS.

    It is used in the Program.cs file to register all the required services for the application.
*/

using Serilog;
using System.Text;
using System.Reflection;
using AspNetCoreRateLimit;
using SafeShare.API.Helpers;
using SafeShare.Security.API;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeShare.Authentication.Auth;
using Microsoft.IdentityModel.Tokens;
using System.Security.Authentication;
using SafeShare.Security.GroupSecurity;
using SafeShare.DataAccessLayer.Context;
using SafeShare.UserManagment.Interfaces;
using Microsoft.Extensions.Configuration;
using SafeShare.Security.User.Interfaces;
using SafeShare.Authentication.Interfaces;
using SafeShare.GroupManagment.Interfaces;
using SafeShare.UserManagment.UserAccount;
using Microsoft.AspNetCore.DataProtection;
using SafeShare.Security.API.ActionFilters;
using SafeShare.Security.User.Implementation;
using SafeShare.ExpenseManagement.Interfaces;
using SafeShare.API.Helpers.AttributeFilters;
using SafeShare.GroupManagment.GroupManagment;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using SafeShare.Security.JwtSecurity.Interfaces;
using SafeShare.ExpenseManagement.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SafeShare.Mappings.SafeShareApi.UserManagment;
using SafeShare.DataAccessLayer.Models.SafeShareApi;
using SafeShare.Mappings.SafeShareApi.Authentication;
using SafeShare.Security.JwtSecurity.Implementations;
using SafeShare.Mappings.SafeShareApi.GroupManagment;
using SafeShare.Mappings.SafeShareApi.ExpenseManagment;
using SafeShare.DataTransormObject.SafeShareApi.Security;
using SafeShare.Utilities.SafeShareApi.ConfigurationSettings;
using SafeShare.DataTransormObject.SafeShareApi.Authentication;
using SafeShare.MediatR.Handlers.CommandsHandlers.Authentication;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

namespace SafeShare.API.Startup;

/// <summary>
/// Entry point for registering all services in the application.
/// </summary>
internal static class API_Helper_ProgramStartup
{
    /// <summary>
    /// Injects all required services into the service collection.
    /// </summary>
    /// <param name="Services">The service collection.</param>
    /// <param name="Configuration">The configuration object.</param>
    /// <param name="host"> The <see cref="IHostBuilder"/></param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection InjectServices
    (
       this IServiceCollection Services,
       IConfiguration Configuration,
       IHostBuilder host
    )
    {
        try
        {
            Services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.IsEssential = true;
            });

            Services.AddScoped<API_Helper_AntiforgeryValidationFilter>();

            Services.AddControllers();
            Services.AddMemoryCache();
            Services.AddEndpointsApiExplorer();
            Services.AddSwaggerGen();
            Services.AddHttpContextAccessor();

            Services.AddDataProtection()
                    .SetApplicationName("SafeShare")
                    .SetDefaultKeyLifetime(TimeSpan.FromDays(7))
                    .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
                    {
                        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
                        ValidationAlgorithm = ValidationAlgorithm.HMACSHA512
                    });

            AddDatabase(Services, Configuration);
            AddConfigurations(Services, Configuration);
            AddAutomapper(Services);
            AddSwagger(Services, Configuration);
            AddServices(Services);
            AddCors(Services, Configuration);
            AddAPIRateLimiting(Services, Configuration);
            AddSerilog(host);
            AddMediatR(Services);
            EnforceTLS(Services);

            return Services;
        }
        catch (Exception)
        {
            throw;
        }
    }
    /// <summary>
    ///     Add configurations in the container
    /// </summary>
    /// <param name="services"> The services collection </param>
    /// <param name="configuration"> The configurations collection </param>
    private static void
    AddConfigurations
    (
        IServiceCollection services,
        IConfiguration configuration
    )
    {
        bool canParseLimitToInt = double.TryParse(configuration.GetSection("DefaultTokenExpirationTimeInHours").Value, out double TokenLifespan);

        if (canParseLimitToInt && TokenLifespan <= 0)
            throw new Exception("Token lifespan can not be 0 or lower");

        services.Configure<Util_JwtSettings>(configuration.GetSection(Util_JwtSettings.SectionName));
        services.Configure<API_Helper_CookieSettings>(configuration.GetSection(API_Helper_CookieSettings.SectionName));
        services.Configure<DataProtectionTokenProviderOptions>
        (
            opt => opt.TokenLifespan = TimeSpan.FromHours(TokenLifespan)
        );
        services.Configure<Util_ResetPasswordSettings>(configuration.GetSection(Util_ResetPasswordSettings.SectionName));
        services.Configure<Util_ActivateAccountSettings>(configuration.GetSection(Util_ActivateAccountSettings.SectionName));
        services.Configure<Util_ChangeEmailAddressSettings>(configuration.GetSection(Util_ChangeEmailAddressSettings.SectionName));
        services.Configure<Util_ConfirmRegistrationSettings>(configuration.GetSection(Util_ConfirmRegistrationSettings.SectionName));
    }
    /// <summary>
    ///     Enfoce the usage of TLS of latest versions
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/></param>
    private static void
    EnforceTLS
    (
        IServiceCollection services
    )
    {
        services.Configure<KestrelServerOptions>(options =>
        {
            options.ConfigureHttpsDefaults(httpsOptions =>
            {
                httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
            });
        });
    }
    /// <summary>
    ///     Add custom services in the container.
    /// </summary>
    /// <param name="Services"> The <see cref="IServiceCollection"/> </param>
    private static void
    AddServices
    (
        IServiceCollection Services
    )
    {
        Services.AddScoped<VerifyUser>();
        Services.AddScoped<IAUTH_Login, AUTH_Login>();
        Services.AddScoped<IAUTH_Register, AUTH_Register>();
        Services.AddScoped<IGroupKeySecurity, GroupKeySecurity>();
        Services.AddScoped<IAccountManagment, AccountManagment>();
        Services.AddScoped<IAUTH_RefreshToken, AUTH_RefreshToken>();
        Services.AddScoped<ISecurity_JwtTokenHash, Security_JwtTokenAuth>();
        Services.AddScoped<IGroupManagment_GroupRepository, GroupManagment_GroupRepository>();
        Services.AddScoped<IGroupManagment_GroupKeyRepository, GroupManagment_GroupKeyRepository>();
        Services.AddScoped<ISecurity_UserDataProtectionService, Security_UserDataProtectionService>();
        Services.AddScoped<IExpenseManagment_ExpenseRepository, ExpenseManagment_ExpenseRepository>();
        Services.AddScoped<IExpenseManagment_SecurityRepository, ExpenseManagment_SecurityRepository>();
        Services.AddScoped<IGroupManagment_GroupInvitationsRepository, GroupManagment_GroupInvitationsRepository>();
        Services.AddScoped<ISecurity_JwtTokenAuth<Security_JwtTokenAuth, DTO_AuthUser, DTO_Token>, Security_JwtTokenAuth>();
        Services.AddScoped<ISecurity_JwtTokenAuth<Security_JwtShortLivedToken, string, string>, Security_JwtShortLivedToken>();
    }
    /// <summary>
    ///     Add sql database with connection string in the container.
    /// </summary>
    /// <param name="Services"> The <see cref="IServiceCollection"/> </param>
    /// <param name="Configuration"> The <see cref="IConfiguration"/> </param>
    private static void
    AddDatabase
    (
        IServiceCollection Services,
        IConfiguration Configuration
    )
    {
        Services.AddDbContext<CryptoKeysDb>(options => options.UseSqlServer(Configuration.GetConnectionString("CryptoKeysConnection")));
        Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        Services.AddDbContext<ApiClientDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));

        //Services.AddDbContext<ApiClientDbContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING")));

        Services.AddIdentity<ApplicationUser, IdentityRole>
        (
            options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            }
        )
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();
    }
    /// <summary>
    ///     Add automaper in the container.
    /// </summary>
    /// <param name="Services"> The <see cref="IServiceCollection"/> </param>
    private static void
    AddAutomapper
    (
        IServiceCollection Services
    )
    {
        Services.AddAutoMapper(typeof(Mapper_GroupManagment));
        Services.AddAutoMapper(typeof(Mapping_Authentication));
        Services.AddAutoMapper(typeof(Mapper_AccountManagment));
        Services.AddAutoMapper(typeof(Mapper_ExpenseManagment));
    }
    /// <summary>
    ///     Add swagger configured with security in the container
    /// </summary>
    /// <param name="Services"> The <see cref="IServiceCollection"/> </param>
    /// <param name="Configuration"> The <see cref="IConfiguration"/> </param>
    private static void
    AddSwagger
    (
        IServiceCollection Services,
        IConfiguration Configuration
    )
    {
        // Services Settings
        var jwtSetting = Configuration.GetSection(Util_JwtSettings.SectionName);

        // Cofigure Authetication
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

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            options.IncludeXmlComments(xmlPath);
        });
    }
    /// <summary>
    ///     Add Cors configured in the container.
    /// </summary>
    /// <param name="Services"> The <see cref="IServiceCollection"/> </param>
    /// <param name="Configuration"> The <see cref="IConfiguration"/> </param>
    private static void
    AddCors
    (
        IServiceCollection Services,
        IConfiguration Configuration
    )
    {
        string? MainClientBaseAddr = Configuration.GetSection("ClientOrigin").Value ?? throw new Exception("ClientOrigin is null");

        string? XSRF_TOKEN_Header = Configuration.GetSection("RequestHeaderSettings:XSRF_TOKEN").Value ?? throw new Exception("XSRF_TOKEN is null");
        string? AspNetCoreAntiforgery_Header = Configuration.GetSection("RequestHeaderSettings:AspNetCoreAntiforgery").Value ?? throw new Exception("AspNetCoreAntiforgery is null");
        string? ClientIP_Header = Configuration.GetSection("RequestHeaderSettings:ClientIP").Value ?? throw new Exception("ClientIP is null");
        string? ApiKey_Header = Configuration.GetSection("RequestHeaderSettings:ApiKey").Value ?? throw new Exception("ApiKey is null");
        string? AuthToken_Header = Configuration.GetSection("RequestHeaderSettings:AuthToken").Value ?? throw new Exception("AuthToken is null");
        string? RefreshAuthToken_Header = Configuration.GetSection("RequestHeaderSettings:RefreshAuthToken").Value ?? throw new Exception("RefreshAuthToken is null");
        string? RefreshAuthTokenId_Header = Configuration.GetSection("RequestHeaderSettings:RefreshAuthTokenId").Value ?? throw new Exception("RefreshAuthTokenId is null");
        string? AspNetCoreIdentity_Header = Configuration.GetSection("RequestHeaderSettings:AspNetCoreIdentity").Value ?? throw new Exception("AspNetCoreIdentity is null");

        string? AllowedMethodGet = Configuration.GetSection("RequestMethodsSettings:Get").Value ?? throw new Exception("RequestMethodsSettings:Get is null");
        string? AllowedMethodPut = Configuration.GetSection("RequestMethodsSettings:Put").Value ?? throw new Exception("RequestMethodsSettings:Put is null");
        string? AllowedMethodPost = Configuration.GetSection("RequestMethodsSettings:Post").Value ?? throw new Exception("RequestMethodsSettings:Post is null");
        string? AllowedMethodDelete = Configuration.GetSection("RequestMethodsSettings:Delete").Value ?? throw new Exception("RequestMethodsSettings:Delete is null");

        string? PolicyName = Configuration.GetSection("Cors:Policy:Name").Value ?? throw new Exception("RequestMethodsSettings:Delete is null");

        Services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, builder =>
            {
                builder.WithOrigins(MainClientBaseAddr)
                       .WithMethods(AllowedMethodGet, AllowedMethodPut, AllowedMethodPost, AllowedMethodDelete)
                       .WithHeaders
                       (
                            XSRF_TOKEN_Header,
                            AspNetCoreAntiforgery_Header,
                            ClientIP_Header,
                            ApiKey_Header,
                            AuthToken_Header,
                            RefreshAuthToken_Header,
                            RefreshAuthTokenId_Header,
                            AspNetCoreIdentity_Header
                        )
                       .AllowCredentials();
            });
        });
    }
    /// <summary>
    ///     Add api rate limiting in the container.
    /// </summary>
    /// <param name="Services"> The <see cref="IServiceCollection"/> </param>
    /// <param name="Configuration"> The <see cref="IConfiguration"/></param>
    private static void
    AddAPIRateLimiting
    (
        IServiceCollection Services,
        IConfiguration Configuration
    )
    {
        bool canParseLimitToInt = int.TryParse(Configuration.GetSection("IpRateLimitOptions:Limit").Value, out int Limit);

        if (canParseLimitToInt && Limit <= 0)
            throw new Exception("Api rate limiting can not be 0 or lower");

        bool canParsePeriodToInt = int.TryParse(Configuration.GetSection("IpRateLimitOptions:PeriodTimespan").Value, out int PeriodTimespan);

        if (canParsePeriodToInt && PeriodTimespan <= 0)
            throw new Exception("Api rate limiting Period Timespan can not be 0 or lower");

        // Limit 100 requests per minute for all endpoints. 
        Services.Configure<IpRateLimitOptions>(options =>
        {
            options.GeneralRules =
            [
                new RateLimitRule
                {
                    Endpoint = "*",
                    Limit = Limit,
                    PeriodTimespan = TimeSpan.FromMinutes(PeriodTimespan)
                }
            ];
        });

        Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
    }
    /// <summary>
    ///     Add serilog in the container
    /// </summary>
    /// <param name="host"> The <see cref="IHostBuilder"/></param>
    private static void
    AddSerilog
    (
        IHostBuilder host
    )
    {
        host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
    }
    /// <summary>
    ///     Add mediatr configuration in the container
    /// </summary>
    /// <param name="services"> The <see cref="IServiceCollection"/> </param>
    private static void
    AddMediatR
    (
        IServiceCollection services
    )
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(MediatR_RegisterUserCommandHandler).GetTypeInfo().Assembly));
    }
}