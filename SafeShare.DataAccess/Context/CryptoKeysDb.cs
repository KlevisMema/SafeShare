using System;
using Microsoft.EntityFrameworkCore;
using SafeShare.DataAccessLayer.Models.SafeShareApiKey;
using SafeShare.DataAccessLayer.Models.SafeShareApi.CryptoModels;

namespace SafeShare.DataAccessLayer.Context;

public class CryptoKeysDb(DbContextOptions<CryptoKeysDb> options) : DbContext(options)
{
    public DbSet<GroupKey> GroupKeys { get; set; }
}