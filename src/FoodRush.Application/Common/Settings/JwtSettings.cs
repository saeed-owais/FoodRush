using System.ComponentModel.DataAnnotations;

namespace FoodRush.Application.Common.Settings
{
    public sealed class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        [Required]
        public string SecretKey { get; init; } = string.Empty;

        [Required]
        public string Issuer { get; init; } = string.Empty;

        [Required]
        public string Audience { get; init; } = string.Empty;

        [Range(1, 1440)]
        public int ExpiryMinutes { get; init; }
    }
}
