using Microsoft.EntityFrameworkCore;
using SafeShare.DataAccessLayer.Models.SafeShareApiKey;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SafeShare.DataAccessLayer.Context;

public class ApiClientDbContext : IdentityDbContext<ApiClient>
{
    public ApiClientDbContext()
    {
    }

    public ApiClientDbContext(DbContextOptions<ApiClientDbContext> options) : base(options)
    {
    }

    public DbSet<ApiClient> Clients { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
}