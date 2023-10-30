using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SafeShare.Utilities.Services;

namespace SafeShare.Utilities.Dependencies;

public class Util_BaseContextDependencies<TContext, TRepository> : Util_BaseDependencies<TRepository>
where TRepository : class
where TContext : class
{
    /// <summary>
    /// Represents the database context
    /// </summary>
    protected readonly TContext _db;
    /// <summary>
    /// Initializes a new instance of the <see cref="Util_BaseContextDependencies{TContext, TRepository}"/> class.
    /// </summary>
    /// <param name="mapper">An instance of IMapper to handle object mappings.</param>
    /// <param name="logger">An instance of ILogger specific to the type T.</param>
    /// <param name="db">An instance of Application db context</param>
    /// <param name="httpContextAccessor">An instance of IHttpContextAccessor to access current HTTP context.</param>
    public Util_BaseContextDependencies
    (
        TContext db,
        IMapper mapper,
        ILogger<TRepository> logger,
        IHttpContextAccessor httpContextAccessor
    )
    : base
    (
        mapper,
        logger,
        httpContextAccessor
    )
    {
        _db = db;
    }
}