using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using SafeShare.DataAccessLayer.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SafeShare.DataAccessLayer.Seeders;

public class RolesSeeder
{
    public static async Task
        SeedRolesAsync
        (
           IApplicationBuilder applicationBuilder,
           IConfiguration configuration
        )
    {
        using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();

        var _context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

        if (_context is not null)
        {
            _context.Database.EnsureCreated();
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(configuration.GetSection("Roles:Admin").Value!))
                await roleManager.CreateAsync(new IdentityRole(configuration.GetSection("Roles:Admin").Value!));
            if (!await roleManager.RoleExistsAsync(configuration.GetSection("Roles:User").Value!))
                await roleManager.CreateAsync(new IdentityRole(configuration.GetSection("Roles:User").Value!));
        }
    }
}