using AuthenticationApi.Application.Dtos;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using BuildingBlocks.Core.Authorization;
using BuildingBlocks.Core.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthenticationDbContext _context;
        private readonly IConfiguration _config;

        public UserRepository(AuthenticationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AppUser> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user is null ? null! : user!;
        }

        public async Task<GetUserDto> GetUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user is not null ? new GetUserDto(user.Id,
                user.Name!,
                user.TelephoneNumber!,
                user.Address!,
                user.Email!,
                user.Role!) : null!;
        }

        public async Task<Response> Register(AppUserDto appUserDto)
        {
            var getUser = await GetUserByEmail(appUserDto.Email);
            //if (getUser is null)
            //    return new Response(false, $"you cannot use this email for registration");
            if (getUser is not null)
            {
                return new Response(false, "Email is already registered");
            }

            var result = _context.Users.Add(new AppUser
            {
                Name = appUserDto.UserName,
                TelephoneNumber = appUserDto.TelephoneNumber,
                Address = appUserDto.Address,
                Email = appUserDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(appUserDto.Password),
                Role = "User"
            });

            await _context.SaveChangesAsync();
            return result.Entity.Id > 0 ? new Response(true, $"User {appUserDto.UserName} registered successfully") 
                : new Response(false, $"User {appUserDto.UserName} registration failed");
        }

        public async Task<Response> Login(LoginDto loginDto)
        {
            var getUser = await GetUserByEmail(loginDto.Email);
            if (getUser is null)
                return new Response(false,"Invalid credentials");

            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, getUser.Password);
            if(!verifyPassword)
                return new Response(false,"Invalid credentials");

            var permissions = await _context.UserPermissions
                .Where(p => p.UserId == getUser.Id)
                .ToListAsync();

            string token = GenerateToken(getUser,permissions,_config);

            //string token = GenerateToken(getUser, _config);

            return new Response(true, token);
        }

        public static string GenerateToken(AppUser user, List<UserPermission> permissions, IConfiguration config)
        {
            var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Name!),
                new(ClaimTypes.Email, user.Email!),
            };
            if (!string.IsNullOrEmpty(user.Role) && !Equals("string", user.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role!));
            }

            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permissions",
                    $"{permission.Feature}:{(int)permission.PermissionLevel}"));
            }

            var token = new JwtSecurityToken(
                issuer: config["Authentication:Issuer"],
                audience: config["Authentication:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Response> AssignPermission(AssignPermissionDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);

            if (user is null)
            {
                return new Response(false, "User not found");
            }

            if (!Enum.IsDefined(dto.PermissionLevel) ||
                dto.PermissionLevel == PermissionLevel.None)
            {
                return new Response(false, "Invalid permission level");
            }

            //If dto.Feature is null, calling .Trim() can throw an exception.

            if (string.IsNullOrWhiteSpace(dto.Feature))
            {
                return new Response(false, "Feature is required");
            }

            var feature = dto.Feature.Trim().ToUpperInvariant();

            if (feature != FeatureConstants.Product &&
                feature != FeatureConstants.Order &&
                feature != FeatureConstants.UserManagement)
            {
                return new Response(false, "Invalid feature");
            }

            //this checks if the user already has a permission for the specified feature.
            //If it does, it updates the permission level; if not, it creates a new permission entry.

            var existingPermission = await _context.UserPermissions 
                .FirstOrDefaultAsync(p =>
                    p.UserId == dto.UserId &&
                    p.Feature == feature);

            if (existingPermission is not null)
            {
                existingPermission.PermissionLevel = dto.PermissionLevel;
            }
            else
            {
                var permission = new UserPermission
                {
                    UserId = dto.UserId,
                    Feature = feature,
                    PermissionLevel = dto.PermissionLevel
                };

                _context.UserPermissions.Add(permission);
            }

            await _context.SaveChangesAsync();

            return new Response(
                true,
                $"Permission assigned successfully for {feature}");
        }
    }
}
