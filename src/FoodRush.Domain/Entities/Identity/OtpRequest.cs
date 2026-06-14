using FoodRush.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodRush.Domain.Entities.Identity
{
    public sealed class OtpRequest : BaseEntity
    {
        public string PhoneNumber { get; set; } = string.Empty;

        public string CodeHash { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public int AttemptCount { get; set; }
        public int ResendCount { get; set; }

        public DateTime? VerifiedAt { get; set; }

        [NotMapped]
        public bool IsVerified => VerifiedAt is not null;
        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    }
}
