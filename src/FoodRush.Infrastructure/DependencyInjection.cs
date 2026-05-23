using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common.Settings;
using FoodRush.Domain.Entities.Identity;
using FoodRush.Infrastructure.Authentication;
using FoodRush.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace FoodRush.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDatabase(configuration)
                .AddJwtAuthentication(configuration)
                .AddAuthorization();

            return services;
        }
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });

            });

            services.AddScoped<IApplicationDbContext>(
                sp => sp.GetRequiredService<ApplicationDbContext>());

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            JwtSettings jwtSettings =
               configuration
                   .GetSection(JwtSettings.SectionName)
                   .Get<JwtSettings>()
               ?? throw new InvalidOperationException(
                   "JWT settings are missing.");

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,

                            ValidateAudience = true,

                            ValidateLifetime = true,

                            ValidateIssuerSigningKey = true,

                            ValidIssuer = jwtSettings.Issuer,

                            ValidAudience = jwtSettings.Audience,

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(
                                        jwtSettings.SecretKey)),

                            ClockSkew = TimeSpan.Zero
                        };

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            IApplicationDbContext dbContext =
                                context.HttpContext.RequestServices
                                    .GetRequiredService<IApplicationDbContext>();

                            ClaimsPrincipal principal =
                                context.Principal!;

                            string? userId =
                                principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                            string? securityStamp =
                                principal.FindFirst("security_stamp")?.Value;

                            if (string.IsNullOrWhiteSpace(userId) ||
                                string.IsNullOrWhiteSpace(securityStamp))
                            {
                                context.Fail("Invalid token.");

                                return;
                            }

                            User? user = await dbContext.Users
                                .AsNoTracking()
                                .FirstOrDefaultAsync(
                                    u => u.Id == Guid.Parse(userId));

                            if (user is null)
                            {
                                context.Fail("Invalid token.");

                                return;
                            }

                            if (user.SecurityStamp != securityStamp)
                            {
                                context.Fail("Token has been revoked.");

                                return;
                            }
                        }
                    };
                });

            services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<IUserContext, UserContext>();
            services.AddScoped<ICurrentRequestInfo, CurrentRequestInfo>();
            services.AddScoped<IRefreshTokenHasher, RefreshTokenHasher>();
            services.AddHttpContextAccessor();
            return services;
        }
    }
}
