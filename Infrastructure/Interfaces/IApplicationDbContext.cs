using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IApplicationDbContext
{
    /// <summary>
    /// DbSet of Users.
    /// </summary>
    DbSet<User> Users { get; }

    /// <summary>
    /// DbSet of Bets.
    /// </summary>
    DbSet<Bet> Bets { get; }

    /// <summary>
    /// Save changes to the database. 
    /// </summary>
    /// <param name="cancellationToken">The cancellation token for the request.</param>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
