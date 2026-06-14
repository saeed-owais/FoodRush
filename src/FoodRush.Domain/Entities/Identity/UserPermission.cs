namespace FoodRush.Domain.Entities.Identity
{
    public class UserPermission
    {
        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public Guid PermissionId { get; set; }

        public Permission Permission { get; set; } = default!;

        public bool IsGranted { get; set; }
    }
}
