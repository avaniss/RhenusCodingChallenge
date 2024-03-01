using Application.Auth;
using Application.Behaviours;
using Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.AddAutoMapper(Assembly.GetExecutingAssembly());
        _ = services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        _ = services.AddMediatR(c => c.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

        _ = services.AddSingleton<ISecurityTokenGenerator>(
            new JwtGenerator(
              key: configuration["Jwt:Key"] ?? "ltvqfLYJlnLeDvZWEjtxsLntdLBrwWBmgp",
              issuer: configuration["Jwt:Issuer"] ?? "Avani Patel",
              audience: configuration["Jwt:Audience"] ?? "Internal Users",
              validityInHours: configuration.GetValue<int>("Jwt:ValidForHours"))
            );

        _ = services.AddScoped<Random>();

        return services;
    }
}
