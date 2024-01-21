using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SafeShare.API.Helpers.AttributeFilters;

public class API_Helper_AntiforgeryValidationFilter(IAntiforgery antiforgery) : IAsyncAuthorizationFilter
{
    public async Task
    OnAuthorizationAsync
    (
        AuthorizationFilterContext context
    )
    {
        await antiforgery.ValidateRequestAsync(context.HttpContext);
    }
}