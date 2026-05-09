using FoodRush.Domain.Common;

namespace FoodRush.Domain.Entities.Identity
{
    public sealed class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;

        public string NormalizedEmail { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string? NormalizedPhoneNumber { get; set; }

        public string PasswordHash { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
            = new List<UserRole>();

        public bool IsEmailVerified { get; set; }

        public bool IsPhoneVerified { get; set; }

        public bool IsActive { get; set; } = true;

        public int FailedLoginAttempts { get; set; }

        public DateTime? LockoutEnd { get; set; }

        public string? FcmToken { get; set; }

        public string SecurityStamp { get; set; }
            = Guid.NewGuid().ToString();

        public DateTime? LastLoginAt { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; }
            = new List<RefreshToken>();
    }
}
