using FoodRush.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodRush.Domain.Entities.Identity
{
    public sealed class RefreshToken : BaseEntity
    {
        public Guid UserId { get; set; }

        public User User { get; set; } = default!;

        public string TokenHash { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public DateTime? UsedAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public string? ReplacedByTokenHash { get; set; }

        public string? JwtId { get; set; }

        public string? CreatedByIp { get; set; }

        public string? RevokedByIp { get; set; }

        public string? UserAgent { get; set; }

        [NotMapped]
        public bool IsExpired =>
            DateTime.UtcNow >= ExpiresAt;

        [NotMapped]
        public bool IsRevoked =>
            RevokedAt is not null;

        [NotMapped]
        public bool IsUsed =>
            UsedAt is not null;

        [NotMapped]
        public bool IsActive =>
            !IsExpired &&
            !IsRevoked &&
            !IsUsed;
    }
}
