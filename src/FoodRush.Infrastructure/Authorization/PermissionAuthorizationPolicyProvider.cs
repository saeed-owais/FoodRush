using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace FoodRush.Infrastructure.Authorization
{
    internal class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _authorizationOptions;
        public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
            : base(options)
        {
            _authorizationOptions = options.Value;
        }

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);

            if (policy != null)
            {
                return policy;
            }

            policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            _authorizationOptions.AddPolicy(policyName, policy);

            return policy;
        }
    }
}
