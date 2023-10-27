using System;
using SafeShare.Utilities.IP;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SafeShare.Utilities.Responses;

namespace SafeShare.Utilities.Log;

public static class Util_LogsHelper<TReturnType, TLogger>
{

    public static async Task<Util_GenericResponse<TReturnType>>
    ReturnInternalServerError
    (
        Exception ex,
        ILogger<TLogger> _logger,
        string errorMessage,
        TReturnType? value,
        IHttpContextAccessor httpContextAccessor

    )
    {
        var errorResponse = Util_GenericResponse<TReturnType>.Response(value, false, ex.ToString(), null, System.Net.HttpStatusCode.InternalServerError);

        _logger.LogError(ex, $"{errorMessage}, [IP] {await Util_GetIpAddres.GetLocation(httpContextAccessor)}", errorResponse);

        errorResponse.Message = "Internal server error";

        return errorResponse;
    }
}