using Domain.Entities;
using Infrastructure.Common;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Bet> Bets => Set<Bet>();

    private readonly IConfiguration _configuration;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        _ = builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
        var initialBalance = _configuration.GetValue<long>("User:InitialBalance");

        if (_configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            _ = builder.Entity<User>().HasData(
            new User("Root User", "root", "Root123!", initialBalance),
            new User("Dev User", "dev", "Dev@123!", initialBalance));

            builder.Entity<User>()
                .Property(u => u.Id)
                .HasValueGenerator<IdGenerator>();

            builder.Entity<Bet>()
                .Property(b => b.Id)
                .HasValueGenerator<IdGenerator>();
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
