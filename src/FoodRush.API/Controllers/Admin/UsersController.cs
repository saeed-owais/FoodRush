using FoodRush.API.Attributes;
using FoodRush.API.Extensions;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Authorization;
using FoodRush.Application.Features.Administration.Users.AssignPermissionToUser;
using FoodRush.Application.Features.Administration.Users.RemovePermissionFromUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FoodRush.API.Controllers.Admin;

[Route("api/admin/[controller]")]
[ApiController]
public class UsersController(IMediator _mediator) : ControllerBase
{
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
}
