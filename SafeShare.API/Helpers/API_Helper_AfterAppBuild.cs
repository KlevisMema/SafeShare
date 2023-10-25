using SafeShare.DataAccessLayer.Seeders;

namespace SafeShare.API.Helpers;

public static class API_Helper_AfterAppBuild
{
    public static async Task Extension
    (
        this WebApplication app,
        IConfiguration Configuration
    )
    {
        // Seed roles 
        await RolesSeeder.SeedRolesAsync(app, Configuration);
    }
}