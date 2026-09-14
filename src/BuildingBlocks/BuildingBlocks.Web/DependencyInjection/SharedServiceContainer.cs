using BuildingBlocks.Web.Authorization;
using BuildingBlocks.Web.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace BuildingBlocks.Web.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>(this IServiceCollection services, IConfiguration config, string fileName) where TContext : DbContext
        {
            //Add Generic Database context

            //services.AddJWTAuthenticationScheme(config);

            //services.AddDbContext<TContext>(option => option.UseSqlServer(
            //    config.
            //    GetConnectionString("eCommerceConnection"), sqlServerOption =>
            //    sqlServerOption.EnableRetryOnFailure()));

            var connectionString = config.GetConnectionString("eCommerceConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception("Connection string 'eCommerceConnection' not found.");
            }

            services.AddDbContext<TContext>(options =>
                options.UseSqlServer(connectionString, sqlServerOption =>
                    sqlServerOption.EnableRetryOnFailure()));

            Directory.CreateDirectory("Logs");

            var logPath = Path.Combine("Logs", $"{fileName}-.log");
            //configure serilog logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.Console()
                //.WriteTo.File($"Logs/{fileName}-.log", 
                .WriteTo.File(logPath,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:Lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Add JWT Authentication Scheme
            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, config);

            // Add Authorization
            services.AddAuthorization();

            // Add Authorization Handler
            services.AddScoped<IAuthorizationHandler, FeaturePermissionHandler>();

            // Add custom authorization policy provider
            services.AddSingleton<IAuthorizationPolicyProvider, FeaturePermissionPolicyProvider>();

            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            // Use Global Exception 
            app.UseMiddleware<GlobalException>();

            // Register Middleware to block outsiders API calls
            app.UseMiddleware<ListenToOnlyApiGateway>();
            return app;
        }
    }
}
