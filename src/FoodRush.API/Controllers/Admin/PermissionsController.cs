using FoodRush.API.Attributes;
using FoodRush.API.Extensions;
using FoodRush.API.ViewModels;
using FoodRush.Domain.Common;
using FoodRush.Application.Common.Authorization;
using FoodRush.Application.Features.Administration.Permissions.CreatePermission;
using FoodRush.Application.Features.Administration.Permissions.DeletePermission;
using FoodRush.Application.Features.Administration.Permissions.GetPermissions;
using FoodRush.Application.Features.Administration.Permissions.UpdatePermission;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodRush.API.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "SUPER_ADMIN")]
    public class PermissionsController(IMediator _mediator) : ControllerBase
    {
        [HttpGet]
        [HasPermission(Permissions.PermissionsManagement.Read)]
        public async Task<IActionResult> GetAllPermissions(CancellationToken cancellationToken)
        {
            Result<List<PermissionResponse>> result =
                await _mediator.Send(new GetPermissionsQuery(), cancellationToken);

            return result.Match(
                onSuccess: permissions => Ok(permissions),
                onFailure: errors => errors.Problem());
        }

        [HttpPost]
        [HasPermission(Permissions.PermissionsManagement.Create)]
        public async Task<IActionResult> CreatePermission(CreatePermissionCommand command, CancellationToken cancellationToken)
        {
            Result<CreatePermissionResponse> result =
                await _mediator.Send(command, cancellationToken);

            return result.Match(
                success => CreatedAtAction(
                    nameof(GetAllPermissions),
                    new { PermissionId = success.Id },
                    success),
                onFailure: errors => errors.Problem());
        }

        [HttpPut("{permissionId:guid}")]
        [HasPermission(Permissions.PermissionsManagement.Update)]
        public async Task<IActionResult> UpdatePermission(Guid permissionId, UpdatePermissionRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdatePermissionCommand(permissionId, request.Name);

            Result<UpdatePermissionResponse> result =
                await _mediator.Send(command, cancellationToken);

            return result.Match(
                success => Ok(success),
                onFailure: errors => errors.Problem());
        }


        [HttpDelete("{permissionId:guid}")]
        [HasPermission(Permissions.PermissionsManagement.Delete)]
        public async Task<IActionResult> DeletePermission(Guid permissionId, CancellationToken cancellationToken)
        {
            var command = new DeletePermissionCommand(permissionId);

            Result result = await _mediator.Send(command, cancellationToken);

            return result.Match(
                NoContent,
                onFailure: errors => errors.Problem());
        }

    }
}

