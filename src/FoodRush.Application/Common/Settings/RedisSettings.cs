using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FoodRush.Application.Common.Settings
{
    public sealed class RedisSettings
    {
        public const string SectionName = "RedisSettings";

        [Required]
        public string ConnectionString { get; init; } = string.Empty;
    }
}
