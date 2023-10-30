/*
 * This class contains helper classes and methods to aid in various operations across the API.
*/

using SafeShare.DataAccessLayer.Seeders;

namespace SafeShare.API.Helpers;

/// <summary>
/// Provides utility methods to be executed post application build.
/// </summary>
public static class API_Helper_AfterAppBuild
{
    /// <summary>
    /// An extension method to perform operations right after the web application is built.
    /// One of its primary functions is to seed roles into the application.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <param name="Configuration">Configuration for the application settings.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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