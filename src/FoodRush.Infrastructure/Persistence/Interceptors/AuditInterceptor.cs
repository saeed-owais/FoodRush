using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FoodRush.Infrastructure.Persistence.Interceptors;

internal sealed class AuditInterceptor(IUserContext userContext)
    : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }

        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext context)
    {
        DateTime utcNow = DateTime.UtcNow;

        Guid? currentUserId =
            userContext.IsAuthenticated
                ? userContext.UserId
                : null;

        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:

                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.CreatedBy = currentUserId;

                    entry.Entity.UpdatedAt = utcNow;
                    entry.Entity.UpdatedBy = currentUserId;

                    break;

                case EntityState.Modified:

                    entry.Entity.UpdatedAt = utcNow;
                    entry.Entity.UpdatedBy = currentUserId;

                    break;
            }
        }
    }
}
