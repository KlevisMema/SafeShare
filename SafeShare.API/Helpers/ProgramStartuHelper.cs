using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataAccessLayer.Context;

namespace SafeShare.API.Startup;

public static class ProgramStartuHelper
{
    /// <summary>
    /// Injects all required services into the service collection.
    /// </summary>
    /// <param name="Services">The service collection.</param>
    /// <param name="Configuration">The configuration object.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection InjectServices
    (
       this IServiceCollection Services,
       IConfiguration Configuration
    )
    {
        Services.AddControllers();
        Services.AddMemoryCache();
        Services.AddEndpointsApiExplorer();

        AddDatabase(Services, Configuration);

        return Services;
    }

    /// <summary>
    ///     Add sql database with connection string in the container.
    /// </summary>
    /// <param name="Services"> The <see cref="IServiceCollection"/> </param>
    /// <param name="Configuration"> The <see cref="IConfiguration"/> </param>
    /// <returns> Configured Database </returns>
    private static IServiceCollection
    AddDatabase
    (
        IServiceCollection Services,
        IConfiguration Configuration
    )
    {
        Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedEmail = false)
                        .AddEntityFrameworkStores<ApplicationDbContext>();

        return Services;
    }
}