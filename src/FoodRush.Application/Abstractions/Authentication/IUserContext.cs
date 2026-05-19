namespace FoodRush.Application.Abstractions.Authentication
{
    public interface IUserContext
    {
        bool IsAuthenticated { get; }

        Guid UserId { get; }

        string? Email { get; }

        IReadOnlyCollection<string> Roles { get; }

        string? SecurityStamp { get; }
    }
}
