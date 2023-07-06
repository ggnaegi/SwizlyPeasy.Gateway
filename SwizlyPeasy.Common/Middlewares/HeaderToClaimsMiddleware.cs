﻿using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SwizlyPeasy.Common.Middlewares
{
    /// <summary>
    ///  Middleware used to map headers transmitted by SwizlyPeasy.Gateway to claims
    /// </summary>
    public class HeaderToClaimsMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// </summary>
        /// <param name="next"></param>
        public HeaderToClaimsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        ///  We are expecting the following headers from SwizlyPeasy.Gateway
        ///  "sub" & "email".
        ///  Of course, it is possible to add more headers. this can be modified (SwizlyPeasy.Common.Auth) 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext httpContext)
        {
            var claims = new List<Claim>();

            //in case we are calling the service directly, without gateway
            if (!httpContext.Request.Headers.ContainsKey("sub"))
            {
                await _next(httpContext);
                return;
            }

            if (httpContext.Request.Headers.TryGetValue("sub", out var values))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, values[0]));
            }

            if (httpContext.Request.Headers.TryGetValue("email", out values))
            {
                claims.Add(new Claim(ClaimTypes.Email, values[0]));
            }

            httpContext.User.AddIdentity(new ClaimsIdentity(claims));
            await _next(httpContext);
        }
    }
}