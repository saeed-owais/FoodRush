using FoodRush.API.Attributes;
using FoodRush.API.Extensions;
using FoodRush.API.ViewModels;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Authorization;
using FoodRush.Application.Features.Administration.Roles;
using FoodRush.Application.Features.Administration.Roles.AssignPermissionToRole;
using FoodRush.Application.Features.Administration.Roles.GetRoleById;
using FoodRush.Application.Features.Administration.Roles.GetRolePermissions;
using FoodRush.Application.Features.Administration.Roles.GetRoles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodRush.API.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "SUPER_ADMIN")]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [HasPermission(Permissions.Roles.Read)]
        public async Task<IActionResult> GetAllRoles()
        {
            Result<List<RoleResponse>> roles = await _mediator.Send(new GetRolesQuery());

            return roles.Match(
                success => Ok(success),
                error => error.Problem());
        }

        [HttpGet("{roleId:guid}")]
        [HasPermission(Permissions.Roles.Read)]
        public async Task<IActionResult> GetRoleById(Guid roleId)
        {
            Result<RoleResponse> role = await _mediator.Send(new GetRoleByIdQuery(roleId));

            return role.Match(
                success => Ok(success),
                error => error.Problem());
        }

        [HttpGet("{roleId:guid}/permissions")]
        [HasPermission(Permissions.Roles.Read)]
        public async Task<IActionResult> GetRolePermissions(Guid roleId)
        {
            Result<List<RolePermissionResponse>> rolePermissions =
                await _mediator.Send(new GetRolePermissionsQuery(roleId));

            return rolePermissions.Match(
                success => Ok(success),
                error => error.Problem());
        }

        [HttpPost("{roleId:guid}/permissions")]
        [HasPermission(Permissions.Roles.Update)]
        public async Task<IActionResult> AssignPermission(
            Guid roleId,
            AssignPermissionRequest request,
            CancellationToken cancellationToken)
        {
            Result result = await _mediator.Send(
                new AssignPermissionToRoleCommand(
                    roleId,
                    request.PermissionId),
                cancellationToken);

            return result.Match(
                NoContent,
                failure => failure.Problem());
        }
    }
}
