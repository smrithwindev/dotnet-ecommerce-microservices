

using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repositories;
using BuildingBlocks.Web.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.NetworkInformation;

namespace AuthenticationApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            //Add database connectivity
            //JWT add Authentication scheme

            SharedServiceContainer.AddSharedServices<AuthenticationDbContext>(services, config, config["MySerilog:FileName"]!);

            //Create Dependency Injection for the services

            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }

        public static IApplicationBuilder UseInfrastructureServices(this IApplicationBuilder app)
        {
            //Register middleware such as:
            //Global exception handling : handler external errors
            //Listen Only To Api Gateway : Block all outsider calls

            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}
