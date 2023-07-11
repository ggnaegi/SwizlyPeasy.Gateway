using Microsoft.AspNetCore.Builder;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Common.Extensions;

public static class CodePageExtensions
{
    /// <summary>
    ///     This is probably a hack to handle
    ///     404 status codes...
    /// </summary>
    /// <param name="app"></param>
    /// <exception cref="NotFoundDomainException"></exception>
    public static void Use404AsException(this IApplicationBuilder app)
    {
        app.UseStatusCodePages(new StatusCodePagesOptions
        {
            HandleAsync = ctx =>
            {
                if (ctx.HttpContext.Response.StatusCode == 404)
                    throw new NotFoundDomainException("Uuuups, path not found...", null);
                return Task.CompletedTask;
            }
        });
    }
}