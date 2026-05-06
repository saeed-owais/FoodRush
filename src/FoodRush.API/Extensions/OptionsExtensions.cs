using FoodRush.Application.Common.Settings;

namespace FoodRush.API.Extensions
{
    public static class OptionsExtensions
    {
        public static IServiceCollection AddOptionsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddOptions<JwtSettings>()
              .Bind(configuration.GetSection(JwtSettings.SectionName))
              .ValidateDataAnnotations()
              .ValidateOnStart();

            services.AddOptions<RedisSettings>()
                .Bind(configuration.GetSection(RedisSettings.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}
