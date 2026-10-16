using AuthenticationApi.Domain.Entities;
using BuildingBlocks.Core.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthenticationApi.Infrastructure.Data
{
    public class AuthenticationDataSeeder
    {
        private readonly AuthenticationDbContext _context;

        private readonly IConfiguration _configuration;

        public AuthenticationDataSeeder(AuthenticationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task SeedAsync()
        {
            var adminEmail = _configuration["Authentication:BootstrapAdmin:Email"];

            var adminPassword = _configuration["Authentication:BootstrapAdmin:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) ||
                string.IsNullOrWhiteSpace(adminPassword))
            {
                throw new InvalidOperationException(
                    "Bootstrap admin credentials are not configured.");
            }

            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);

            if (admin is null)
            {
                admin = new AppUser
                {
                    Name = "Bootstrap Admin",
                    Email = adminEmail,
                    Password = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    Role = "Admin",
                    DateRegistered = DateTime.UtcNow
                };

                _context.Users.Add(admin);

                // Save changes to get the admin user ID
                await _context.SaveChangesAsync();
            }

            var requiredPermissions = new[]
            {
            FeatureConstants.Product,
            FeatureConstants.Order,
            FeatureConstants.UserManagement
            };

            // check if the admin user has all required permissions in the UserPermissions table 

            foreach (var feature in requiredPermissions)
            {
                var permissionExists = await _context.UserPermissions
                    .AnyAsync(p =>
                        p.UserId == admin.Id &&
                        p.Feature == feature);

                if (!permissionExists)
                {
                    _context.UserPermissions.Add(
                        new UserPermission
                        {
                            UserId = admin.Id,
                            Feature = feature,
                            PermissionLevel = PermissionLevel.FullAccess
                        });
                }
            }

            await _context.SaveChangesAsync();
        }

    }
}
