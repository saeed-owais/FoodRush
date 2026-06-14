using Microsoft.AspNetCore.Authorization;

namespace FoodRush.API.Attributes;

internal class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission) : base(policy: permission)
    {
    }
}

