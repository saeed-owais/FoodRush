using FoodRush.API.Attributes;
using FoodRush.API.Extensions;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Authorization;
using FoodRush.Application.Features.Administration.Permissions.GetPermissions;
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
        public async Task<IActionResult> GetAllPermissions()
        {
            Result<List<PermissionResponse>> result = await _mediator.Send(new GetPermissionsQuery());

            return result.Match(
                onSuccess: permissions => Ok(permissions),
                onFailure: errors => errors.Problem());
        }
    }
}

