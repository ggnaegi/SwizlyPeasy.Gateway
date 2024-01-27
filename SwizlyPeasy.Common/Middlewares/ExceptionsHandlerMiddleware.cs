using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Common.Middlewares;

/// <summary>
///     Middleware for exception handling, returning
///     user friendly exceptions, according to the RFC 7807 standard.
/// </summary>
public class ExceptionsHandlerMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var rfc7807Exception = Rfc7807.ExceptionFactory(e, context.Request.Path);
            response.StatusCode = rfc7807Exception.Status;

            var result = JsonConvert.SerializeObject(rfc7807Exception);
            await response.WriteAsync(result);
        }
    }
}