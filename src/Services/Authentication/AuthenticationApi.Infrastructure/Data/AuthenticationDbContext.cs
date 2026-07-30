using AuthenticationApi.Domain.Entities;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Infrastructure.Data
{
    public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : DbContext(options)
    {
        public DbSet<AppUser> Users { get; set; }
        }
}
