namespace FoodRush.Application.Features.Administration.Users.GetUserRoles;

public sealed record GetUserRoleResponse(
    Guid Id,
    string Name,
    string Code);