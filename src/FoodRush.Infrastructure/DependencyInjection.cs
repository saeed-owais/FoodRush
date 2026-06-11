using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Notifications;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Abstractions.Storage;
using FoodRush.Application.Common.Settings;
using FoodRush.Domain.Entities.Identity;
using FoodRush.Infrastructure.Authentication;
using FoodRush.Infrastructure.Authorization;
using FoodRush.Infrastructure.BackgroundJobs;
using FoodRush.Infrastructure.Notifications;
using FoodRush.Infrastructure.Persistence;
using FoodRush.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services
                .AddDatabase(connectionString)
                .AddJwtAuthentication(configuration)
                .AddBackgroundJobs(connectionString)
                .AddAuthorization()
                .AddJwtAuthorization()
                .AddNotifications();

            services.Configure<FrontendSettings>(configuration.GetSection(FrontendSettings.SectionName));

            services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));

            services.AddScoped<IFileStorageService, CloudinaryFileStorageService>();

            return services;
        }
        public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
        {


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

                options.EnableDetailedErrors();

                options.EnableSensitiveDataLogging();
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

                            if (!Guid.TryParse(userId, out Guid parsedUserId))
                            {
                                context.Fail("Invalid token.");
                                return;
                            }

                            User? user = await dbContext.Users
                                .AsNoTracking()
                                .FirstOrDefaultAsync(
                                    u => u.Id == parsedUserId);

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

        public static IServiceCollection AddJwtAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization();

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.AddScoped<PermissionsProvider>();

            return services;
        }

        public static IServiceCollection AddNotifications(this IServiceCollection services)
        {
            services.AddDataProtection();

            services.AddScoped<IEmailVerificationTokenProvider, EmailVerificationTokenProvider>();

            services.AddScoped<IPasswordResetTokenProvider, PasswordResetTokenProvider>();

            services.AddScoped<IEmailChangeTokenProvider, EmailChangeTokenProvider>();

            //services.AddKeyedScoped<IEmailService, FakeEmailService>("FakeEmailService");

            services.AddScoped<IEmailService, FakeEmailService>();

            return services;
        }
    }
}
