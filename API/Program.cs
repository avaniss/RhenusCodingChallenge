using API.Middlewares;
using FastEndpoints;
using FastEndpoints.Swagger;
using Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    _ = builder.Configuration
    .AddJsonFile($"{AppDomain.CurrentDomain.BaseDirectory}/appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"{AppDomain.CurrentDomain.BaseDirectory}/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
}

// Add services to the container.
_ = builder.Services.AddApplicationServices(builder.Configuration);
_ = builder.Services.AddInfrastructureServices(builder.Configuration);
_ = builder.Services.AddApiServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
_ = builder.Services.SwaggerDocument(x => x.AutoTagPathSegmentIndex = 0);

var app = builder.Build();
_ = app.UseMiddleware<ExceptionHandlingMiddleware>();
_ = app.UseAuthentication();
_ = app.UseAuthorization();
_ = app.UseFastEndpoints();

_ = app.UseDefaultFiles();
_ = app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    try
    {
        _ = app.UseSwaggerGen();
    }
    catch (Exception) { }
}

_ = app.UseHttpsRedirection();

_ = app.MapFallbackToFile("/index.html");

using (var scope = app.Services.CreateScope())
{
    var dbContextInitializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
    await dbContextInitializer.InitialiseAsync();
}

app.Run();

public partial class Program { }