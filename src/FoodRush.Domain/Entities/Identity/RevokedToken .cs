using FoodRush.Domain.Common;

namespace FoodRush.Domain.Entities.Identity
{
    public sealed class RevokedToken : BaseEntity
    {
        public string JwtId { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public DateTime RevokedAt { get; set; }
    }
}
