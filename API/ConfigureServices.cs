using API.Middlewares;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddSingleton<ExceptionHandlingMiddleware>();
        _ = services.AddHealthChecks();
        _ = services.AddJWTBearerAuth(tokenSigningKey: configuration["Jwt:Key"]);
        _ = services.AddAuthorization();
        _ = services.AddFastEndpoints();

        // Customise default API behaviour
        _ = services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);

        return services;
    }
}
