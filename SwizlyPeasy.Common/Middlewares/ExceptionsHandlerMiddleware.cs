using Microsoft.AspNetCore.Http;
using SwizlyPeasy.Common.Exceptions;
using Newtonsoft.Json;

namespace SwizlyPeasy.Common.Middlewares
{
    /// <summary>
    /// Middleware for exception parsing, returning
    /// user friendly exceptions.
    /// </summary>
    public class ExceptionsHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionsHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception e)
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
}
