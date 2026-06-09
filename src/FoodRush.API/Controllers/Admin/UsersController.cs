using FoodRush.API.Attributes;
using FoodRush.API.Extensions;
using FoodRush.API.ViewModels;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Authorization;
using FoodRush.Application.Common.Models;
using FoodRush.Application.Features.Administration.Users.AssignPermissionToUser;
using FoodRush.Application.Features.Administration.Users.AssignRoleToUser;
using FoodRush.Application.Features.Administration.Users.BanUser;
using FoodRush.Application.Features.Administration.Users.GetUserById;
using FoodRush.Application.Features.Administration.Users.GetUserRoles;
using FoodRush.Application.Features.Administration.Users.GetUsers;
using FoodRush.Application.Features.Administration.Users.RemovePermissionFromUser;
using FoodRush.Application.Features.Administration.Users.RemoveRoleFromUser;
using FoodRush.Application.Features.Administration.Users.UnBanUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FoodRush.API.Controllers.Admin;

[Route("api/admin/[controller]")]
[ApiController]
public class UsersController(IMediator _mediator) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permissions.Users.Read)]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query, CancellationToken cancellationToken)
    {
        Result<PaginatedResponse<GetUsersResponse>> result =
            await _mediator.Send(query, cancellationToken);

        return result.Match(
            Ok,
            failure => failure.Problem());
    }

    [HttpGet("{userId:guid}")]
    [HasPermission(Permissions.Users.Read)]
    public async Task<IActionResult> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        Result<GetUserByIdResponse> result = await _mediator.Send(new GetUserByIdQuery(userId), cancellationToken);

        return result.Match(
            Ok,
            failure => failure.Problem());
    }

    [HttpGet("{userId:guid}/roles")]
    [HasPermission(Permissions.UserRoles.Read)]
    public async Task<IActionResult> GetUserRoles(Guid userId, CancellationToken cancellationToken)
    {
        Result<IReadOnlyCollection<GetUserRoleResponse>> result =
        await _mediator.Send(
            new GetUserRolesQuery(userId),
            cancellationToken);

        return result.Match(
            Ok,
            failure => failure.Problem());
    }

    [HttpPost("{userId:guid}/roles/{roleId:guid}")]
    [HasPermission(Permissions.UserRoles.Assign)]
    public async Task<IActionResult> AssignRoleToUser(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        Result result = await _mediator.Send(new AssignRoleToUserCommand(userId, roleId), cancellationToken);

        return result.Match(
            NoContent,
            failure => failure.Problem());
    }

    [HttpDelete("{userId:guid}/roles/{roleId:guid}")]
    [HasPermission(Permissions.UserRoles.Remove)]
    public async Task<IActionResult> RemoveRoleFromUser(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        Result result = await _mediator.Send(new RemoveRoleFromUserCommand(userId, roleId), cancellationToken);

        return result.Match(
            NoContent,
            failure => failure.Problem());
    }

    [HttpPost("{userId:guid}/permissions/{permissionId:guid}")]
    [HasPermission(Permissions.UserPermissions.Assign)]
    public async Task<IActionResult> AssignPermissionToUser(Guid userId, Guid permissionId, CancellationToken cancellationToken)
    {
        Result result = await _mediator.Send(new AssignPermissionToUserCommand(userId, permissionId));

        return result.Match(
            NoContent,
            failure => failure.Problem());
    }

    [HttpDelete("{userId:guid}/permissions/{permissionId:guid}")]
    [HasPermission(Permissions.UserPermissions.Remove)]
    public async Task<IActionResult> RemovePermissionFromUser(Guid userId, Guid permissionId, CancellationToken cancellationToken)
    {
        Result result = await _mediator.Send(
            new RemovePermissionFromUserCommand(
                userId,
                permissionId),
            cancellationToken);

        return result.Match(
            NoContent,
            failure => failure.Problem());
    }

    [HttpPost("{userId}/ban")]
    [HasPermission(Permissions.Users.Update)]
    public async Task<IActionResult> BanUser(Guid userId, [FromBody] BanUserRequest banUserRequest, CancellationToken cancellationToken)
    {
        Result result = await _mediator.Send(new BanUserCommand(
            userId,
            banUserRequest.BanEndDate), cancellationToken);

        return result.Match(
            NoContent,
            failure => failure.Problem());

    }

    [HttpPost("{userId}/unban")]
    [HasPermission(Permissions.Users.Update)]
    public async Task<IActionResult> UnBanUser(Guid userId, CancellationToken cancellationToken)
    {
        Result result = await _mediator.Send(new UnBanUserCommand(userId), cancellationToken);

        return result.Match(
            NoContent,
            failure => failure.Problem());
    }
}


