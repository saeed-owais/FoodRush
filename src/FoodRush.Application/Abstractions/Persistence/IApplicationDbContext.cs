using FoodRush.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Abstractions.Persistence
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }

        DbSet<RefreshToken> RefreshTokens { get; }

        DbSet<RevokedToken> RevokedTokens { get; }

        DbSet<OtpRequest> OtpRequests { get; }

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
