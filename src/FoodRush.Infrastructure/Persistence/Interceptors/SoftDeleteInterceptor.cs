using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FoodRush.Infrastructure.Persistence.Interceptors;

internal sealed class SoftDeleteInterceptor(IUserContext userContext)
    : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            UpdateSoftDeleteEntities(eventData.Context);
        }

        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

    private void UpdateSoftDeleteEntities(DbContext context)
    {
        DateTime utcNow = DateTime.UtcNow;

        Guid? currentUserId =
            userContext.IsAuthenticated
                ? userContext.UserId
                : null;

        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State != EntityState.Deleted)
            {
                continue;
            }

            entry.State = EntityState.Modified;

            entry.Entity.IsDeleted = true;
            entry.Entity.DeletedAt = utcNow;
            entry.Entity.DeletedBy = currentUserId;

            if (entry.Entity is IAuditable auditable)
            {
                auditable.UpdatedAt = utcNow;
                auditable.UpdatedBy = currentUserId;
            }
        }
    }
}