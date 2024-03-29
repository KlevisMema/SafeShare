﻿/*
 * Defines the seeder for initializing the databases in the application.
*/

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using SafeShare.DataAccessLayer.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SafeShare.DataAccessLayer.Seeders;

/// <summary>
/// Defines the role seeder for initializing default roles in the application.
/// Seeds/created the db of <see cref="ApplicationDbContext"/> and <see cref="CryptoKeysDb"/>
/// </summary>
public class Seeder
{
    /// <summary>
    /// Seeds roles into the application based on the configuration.
    /// Created the db for crypto keys also.
    /// </summary>
    /// <param name="applicationBuilder">The application builder.</param>
    /// <param name="configuration">The application configuration.</param>
    public static async Task
    Seed
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

        var _cryptoKeysDb = serviceScope.ServiceProvider.GetService<CryptoKeysDb>();
        _cryptoKeysDb?.Database.EnsureCreated();
    }
}