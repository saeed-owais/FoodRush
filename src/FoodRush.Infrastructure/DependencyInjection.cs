using Amazon.S3;
using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Notifications;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Abstractions.Persistence.Queries;
using FoodRush.Application.Abstractions.Storage;
using FoodRush.Application.Common.Settings;
using FoodRush.Domain.Restaurants;
using FoodRush.Infrastructure.Authentication;
using FoodRush.Infrastructure.Authorization;
using FoodRush.Infrastructure.BackgroundJobs;
using FoodRush.Infrastructure.Notifications;
using FoodRush.Infrastructure.Notifications.Templates;
using FoodRush.Infrastructure.Persistence;
using FoodRush.Infrastructure.Persistence.Queries;
using FoodRush.Infrastructure.Persistence.Repositories;
using FoodRush.Infrastructure.Resilience;
using FoodRush.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using System.Security.Claims;
using System.Text;

namespace FoodRush.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var redisConnectionString = configuration.GetSection(RedisSettings.SectionName)
            .Get<RedisSettings>()?.ConnectionString
            ?? throw new InvalidOperationException("Redis connection string not found.");

        services
            .AddDatabase(connectionString)
            .AddJwtAuthentication(configuration)
            .AddBackgroundJobs(connectionString)
            .AddAuthorization()
            .AddJwtAuthorization()
            .AddNotifications(configuration)
            .AddStorageServices(configuration)
            .AddResilience()
            .AddDapperQueries();

        services.Configure<FrontendSettings>(configuration.GetSection(FrontendSettings.SectionName));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
        });

        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(30)
            };
        });

        services.AddScoped<IRestaurantRepository, RestaurantRepository>();

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

                        IUserSecurityStampService securityStampService =
                            context.HttpContext.RequestServices
                                .GetRequiredService<IUserSecurityStampService>();

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

                        string? cachedStamp = await securityStampService.GetAsync(parsedUserId);

                        if (cachedStamp != securityStamp)
                        {
                            context.Fail("Token revoked.");
                        }
                    }
                };
            });

        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<ICurrentRequestInfo, CurrentRequestInfo>();
        services.AddScoped<IRefreshTokenHasher, RefreshTokenHasher>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUserSecurityStampService, UserSecurityStampService>();
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

    public static IServiceCollection AddNotifications(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataProtection();

        services.AddScoped<IEmailVerificationTokenProvider, EmailVerificationTokenProvider>();

        services.AddScoped<IPasswordResetTokenProvider, PasswordResetTokenProvider>();

        services.AddScoped<IEmailChangeTokenProvider, EmailChangeTokenProvider>();

        //services.AddKeyedScoped<IEmailService, FakeEmailService>("FakeEmailService");

        services.AddScoped<IEmailService, FakeEmailService>();

        services.AddScoped<IEmailTemplateRenderer, EmailTemplateRenderer>();

        services.AddScoped<IEmailService, SendGridEmailService>();

        services.Configure<SendGridSettings>(configuration.GetSection(SendGridSettings.SectionName));

        SendGridSettings sendGridSettings =
            configuration
                .GetSection(SendGridSettings.SectionName)
                .Get<SendGridSettings>()!;

        services.AddSingleton<ISendGridClient>(
            new SendGridClient(
                sendGridSettings.ApiKey));

        return services;
    }

    public static IServiceCollection AddStorageServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.SectionName));

        services.Configure<CloudflareR2Settings>(configuration.GetSection(CloudflareR2Settings.SectionName));

        services.AddScoped<IFileStorageService, CloudinaryFileStorageService>();

        services.AddScoped<IDocumentStorageService, CloudflareR2DocumentStorageService>();

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<CloudflareR2Settings>>().Value;

            var config = new AmazonS3Config
            {
                ServiceURL = settings.Endpoint,
                ForcePathStyle = true,
                AuthenticationRegion = "auto"
            };

            return new AmazonS3Client(
                settings.AccessKey,
                settings.SecretKey,
                config);
        });

        return services;
    }

    public static IServiceCollection AddDapperQueries(this IServiceCollection services)
    {
        services.AddScoped<IRestaurantQueries, RestaurantQueries>();
        services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

        return services;
    }
}
