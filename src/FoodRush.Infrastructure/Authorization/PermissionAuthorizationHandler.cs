using Microsoft.AspNetCore.Authorization;

namespace FoodRush.Infrastructure.Authorization
{
    internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            bool hasPermission = context.User.Claims
             .Any(c =>
                 c.Type == "permission" &&
                 c.Value == requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}
