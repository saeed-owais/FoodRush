using FoodRush.Domain.Common;

namespace FoodRush.Domain.Entities.Identity
{
    public sealed class Role : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public ICollection<UserRole> UserRoles { get; set; }
            = new List<UserRole>();

        public ICollection<RolePermission> RolePermissions { get; set; }
            = new List<RolePermission>();
    }
}
