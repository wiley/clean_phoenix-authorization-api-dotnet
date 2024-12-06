using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Authorization.Services.Authentication
{
    public class ApiKeyValidationMiddleware
    {
        private readonly RequestDelegate _next;
        public ApiKeyValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
                var authorize = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>();

                if (authorize != null)
                {
                    if (context.Request.Headers.TryGetValue("x-api-key", out var apiKeyHeader))
                    {
                        var expectedApiKey = Environment.GetEnvironmentVariable("API_KEY");

                        if (string.Equals(apiKeyHeader, expectedApiKey))
                        {
                            context.User = new GenericPrincipal(new GenericIdentity("ApiKey", "ApiKey"), null);
                        }
                        else
                        {
                            context.Response.StatusCode = 401;
                            await context.Response.WriteAsync("No valid credentials were provided.");
                            return;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("No valid credentials were provided.");
                        return;
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Invalid Token");
            }
            await _next(context);
        }
    }

    public static class ApiKeyValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyValidation(
            this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ApiKeyValidationMiddleware>();

            return builder;
        }
    }
}