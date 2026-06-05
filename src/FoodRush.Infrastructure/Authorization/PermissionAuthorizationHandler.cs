using FoodRush.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace FoodRush.Infrastructure.Authorization
{
    internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)

        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            PermissionsProvider permissionsProvider = _serviceScopeFactory
                .CreateScope()
                .ServiceProvider
                .GetRequiredService<PermissionsProvider>();

            if (!context.User?.Identity?.IsAuthenticated ?? false)
                return;

            Guid userId = context.User.GetUserId();

            HashSet<string> permissions = await permissionsProvider.GetUserPermissions(userId);

            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }
    }
}
