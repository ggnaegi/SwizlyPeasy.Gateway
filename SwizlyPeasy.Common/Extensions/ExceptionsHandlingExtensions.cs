using Microsoft.AspNetCore.Builder;
using SwizlyPeasy.Common.Middlewares;

namespace SwizlyPeasy.Common.Extensions;

public static class ExceptionsHandlingExtensions
{
    public static void UseSwizlyPeasyExceptions(this WebApplication app)
    {
        app.UseMiddleware<ExceptionsHandlerMiddleware>();
        app.Use404AsException();
    }
}