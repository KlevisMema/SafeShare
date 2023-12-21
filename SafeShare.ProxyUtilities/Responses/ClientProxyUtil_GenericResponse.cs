﻿
using System.Net;

namespace SafeShare.ProxyUtilities.Responses;

public class ClientProxyUtil_GenericResponse<T>
{
    /// <summary>
    ///     A <see cref="string"/> custom message describing an action succsess or fail  etc. 
    ///     It's nullable.
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    ///     A <see cref="bool"/>, indicating if an operation was succsess or not.
    /// </summary>
    public bool Succsess { get; set; }
    /// <summary>
    ///     A <see cref="HttpStatusCode"/>, indicatig the operation status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }
    /// <summary>
    ///     A <see cref="T"/> obejct of any type.
    /// </summary>
    public T? Value { get; set; }
    /// <summary>
    ///     A <see cref="List{T}"/> of string indicating errors.
    /// </summary>
    public List<string>? Errors { get; set; }

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="message"> Message <see cref="string"/> value, it's nullable </param>
    /// <param name="succsess"> Succsess <see cref="bool"/> value </param>
    /// <param name="statusCode"> Status code <see cref="HttpStatusCode"/> value </param>
    /// <param name="value"> Object <see cref="T"/> value, it's nullable </param>
    public ClientProxyUtil_GenericResponse
    (
        string? message,
        bool succsess,
        HttpStatusCode statusCode,
        T? value,
        List<string>? errors
    )
    {
        Message = message;
        Succsess = succsess;
        StatusCode = statusCode;
        Value = value;
        Errors = errors;
    }

    /// <summary>
    ///  A default constructor.
    /// </summary>
    public ClientProxyUtil_GenericResponse()
    {
    }

    /// <summary>
    ///     It creates a new Util_GenericResponse using the costructor with all the fields.
    /// </summary>
    /// <param name="message"> Message <see cref="string"/> value, it's nullable </param>
    /// <param name="succsess"> Succsess <see cref="bool"/> value </param>
    /// <param name="statusCode"> Status code <see cref="HttpStatusCode"/> value </param>
    /// <param name="Value"> Object <see cref="T"/> value, it's nullable </param>
    /// <param name="errors"> A list of errors </param>
    /// <returns> <see cref="Util_GenericResponse{T}"/> </returns>
    public static ClientProxyUtil_GenericResponse<T>
    Response
    (
        T? Value,
        bool succsess,
        string? message,
        List<string>? errors,
        HttpStatusCode statusCode
    )
    {
        return new ClientProxyUtil_GenericResponse<T>(message, succsess, statusCode, Value, errors);
    }
}