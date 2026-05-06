using FoodRush.API.Middleware;

namespace FoodRush.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestContextLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestContextLoggingMiddleware>();
        }
    }
}
