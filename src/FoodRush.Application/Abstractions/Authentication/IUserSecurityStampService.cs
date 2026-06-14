namespace FoodRush.Application.Abstractions.Authentication;

public interface IUserSecurityStampService
{
    Task<string?> GetAsync(Guid userId, CancellationToken cancellationToken = default);

    Task SetAsync(Guid userId, string securityStamp, CancellationToken cancellationToken = default);

}
