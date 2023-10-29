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
using SafeShare.Security.API;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeShare.Authentication.Auth;
using Microsoft.IdentityModel.Tokens;
using SafeShare.Security.JwtSecurity;
using System.Security.Authentication;
using SafeShare.DataAccessLayer.Models;
using SafeShare.Mappings.UserManagment;
using SafeShare.Mappings.Authentication;
using SafeShare.DataAccessLayer.Context;
using SafeShare.UserManagment.Interfaces;
using SafeShare.Authentication.Interfaces;
using SafeShare.UserManagment.UserAccount;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SafeShare.MediatR.Handlers.CommandsHandlers.Authentication;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace SafeShare.API.Startup;

/// <summary>
/// Entry point for registering all services in the application.
/// </summary>
public static class API_Helper_ProgramStartup
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
        Services.AddControllers();
        Services.AddMemoryCache();
        Services.AddEndpointsApiExplorer();
        Services.AddSwaggerGen();

        AddDatabase(Services, Configuration);
        AddAutomapper(Services);
        AddSwagger(Services, Configuration);
        AddServices(Services);
        AddCors(Services, Configuration);
        AddAPIRateLimiting(Services);
        AddSerilog(host);
        AddMediatR(Services);
        EnforceTLS(Services);

        return Services;
    }
    /// <summary>
    ///     Enfoce the usage of TLS of latest versions
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/></param>
    public static void
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
        Services.AddTransient<IAUTH_Login, AUTH_Login>();
        Services.AddTransient<IAUTH_Register, AUTH_Register>();
        Services.AddTransient<IAccountManagment, AccountManagment>();
        Services.AddTransient<ISecurity_JwtTokenAuth, Security_JwtTokenAuth>();
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
        Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        Services.AddIdentity<ApplicationUser, IdentityRole>
            (
                options =>
                {
                    //options.SignIn.RequireConfirmedEmail = false;
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequiredLength = 6;
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
        Services.AddAutoMapper(typeof(Mapping_Authentication));
        Services.AddAutoMapper(typeof(Mapper_AccountManagment));
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
        Services.Configure<Security_JwtSettings>(Configuration.GetSection(Security_JwtSettings.SectionName));
        var jwtSetting = Configuration.GetSection(Security_JwtSettings.SectionName);

        // Add the custom auth filter, a filter that is used by all enpoints
        Services.AddScoped<IApiKeyAuthorizationFilter>(provider =>
        {
            var config = provider.GetService<IConfiguration>();
            string apikey = config!.GetValue<string>("API_KEY")!;
            return new ApiKeyAuthorizationFilter(apikey);
        });
        // Cofigure Authetication
        Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        })
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateAudience = Configuration.GetValue<bool>("Jwt:ValidateAudience"),
                   ValidateIssuer = Configuration.GetValue<bool>("Jwt:ValidateIssuer"),
                   ValidateLifetime = Configuration.GetValue<bool>("Jwt:ValidateLifetime"),
                   ValidateIssuerSigningKey = Configuration.GetValue<bool>("Jwt:ValidateIssuerSigningKey"),
                   ValidIssuer = jwtSetting.GetSection("Issuer").Value,
                   ValidAudience = jwtSetting.GetSection("Audience").Value,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.GetSection("Key").Value!)),
               };
           });

        Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(Configuration.GetSection("Swagger:ApplicationAuth:SecurityDefinition:Definition").Value, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = Configuration.GetSection("Swagger:ApplicationAuth:SecurityDefinition:Name").Value,
                Type = SecuritySchemeType.ApiKey,
                Description = Configuration.GetSection("Swagger:ApplicationAuth:SecurityDefinition:Description").Value
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = Configuration.GetSection("Swagger:ApplicationAuth:SecurityRequirement:Id").Value },
                            Name = Configuration.GetSection("Swagger:ApplicationAuth:SecurityRequirement:Name").Value,
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

            options.AddSecurityDefinition(Configuration.GetSection("Swagger:JwtAuth:SecurityDefinition:Definition").Value, new OpenApiSecurityScheme
            {
                Description = Configuration.GetSection("Swagger:JwtAuth:SecurityDefinition:Description").Value,
                Name = Configuration.GetSection("Swagger:JwtAuth:SecurityDefinition:Name").Value,
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = Configuration.GetSection("Swagger:JwtAuth:SecurityDefinition:Scheme").Value,
                BearerFormat = Configuration.GetSection("Swagger:JwtAuth:SecurityDefinition:BearerFormat").Value
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = Configuration.GetSection("Swagger:JwtAuth:SecurityRequirement:Reference:Id").Value
                            },
                            Scheme = Configuration.GetSection("Swagger:JwtAuth:SecurityRequirement:Scheme").Value,
                            Name = Configuration.GetSection("Swagger:JwtAuth:SecurityRequirement:Name").Value,
                            In = ParameterLocation.Header,
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
        Services.AddCors(options =>
        {
            options.AddPolicy(Configuration.GetSection("Cors:Policy:Name").Value!, builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
        });
    }
    /// <summary>
    ///     Add api rate limiting in the container.
    /// </summary>
    /// <param name="Services"> The <see cref="IServiceCollection"/> </param>
    private static void
    AddAPIRateLimiting
    (
        IServiceCollection Services
    )
    {
        // Limit 100 requests per minute for all endpoints. 
        Services.Configure<IpRateLimitOptions>(options =>
        {
            options.GeneralRules = new List<RateLimitRule>
                    {
                        new RateLimitRule
                        {
                            Endpoint = "*",
                            Limit = 100,
                            PeriodTimespan = TimeSpan.FromMinutes(1)
                        }
                    };
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