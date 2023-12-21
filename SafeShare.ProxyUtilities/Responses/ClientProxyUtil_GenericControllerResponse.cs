using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SafeShare.ProxyUtilities.Responses;

public class ClientProxyUtil_GenericControllerResponse<T> : ControllerBase
{
    /// <summary>
    ///     Return a object result with <see cref="Util_GenericResponse{T}"/> as object parameter depending from the service <see cref="HttpStatusCode"/> response.
    /// </summary>
    /// <param name="obj"> A <see cref="Util_GenericResponse{T}"/> object </param>
    /// <returns> <see cref="ObjectResult"/> </returns>
    public static ObjectResult ControllerResponse(ClientProxyUtil_GenericResponse<T> obj)
    {
        return obj.StatusCode switch
        {
            HttpStatusCode.NotFound => new NotFoundObjectResult(obj),
            HttpStatusCode.BadRequest => new BadRequestObjectResult(obj),
            HttpStatusCode.OK => new OkObjectResult(obj),
            HttpStatusCode.Forbidden => new BadRequestObjectResult(obj),
            _ => new ObjectResult(obj) { StatusCode = StatusCodes.Status500InternalServerError },
        };
    }
    /// <summary>
    ///     Return a object result with <see cref="ClientProxyUtil_GenericResponse{T}"/> as object parameter depending from the service <see cref="HttpStatusCode"/> response.
    /// </summary>
    /// <param name="obj">List of object that will come from a controller</param>
    /// <returns>The appropriate status code</returns>
    public static ObjectResult ControllerResponseList(ClientProxyUtil_GenericResponse<List<T>> obj)
    {
        return obj.StatusCode switch
        {
            HttpStatusCode.NotFound => new NotFoundObjectResult(obj),
            HttpStatusCode.BadRequest => new BadRequestObjectResult(obj),
            HttpStatusCode.OK => new OkObjectResult(obj),
            HttpStatusCode.Forbidden => new BadRequestObjectResult(obj),
            _ => new ObjectResult(obj) { StatusCode = StatusCodes.Status500InternalServerError },
        };
    }
}