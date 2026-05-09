using FoodRush.Domain.Common;

namespace FoodRush.Domain.Entities.Identity
{
    public sealed class Permission : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public ICollection<RolePermission> RolePermissions { get; set; }
            = new List<RolePermission>();
    }
}
