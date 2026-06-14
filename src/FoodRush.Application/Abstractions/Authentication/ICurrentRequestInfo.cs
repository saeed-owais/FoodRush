namespace FoodRush.Application.Abstractions.Authentication;

public interface ICurrentRequestInfo
{
    string? IpAddress { get; }

    string? UserAgent { get; }
}

