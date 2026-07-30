using AuthenticationApi.Application.Dtos;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
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
            if (getUser is null)
                return new Response(false, $"you cannot use this email for registration");

            var result = _context.Users.Add(new AppUser
            {
                Name = appUserDto.UserName,
                TelephoneNumber = appUserDto.TelephoneNumber,
                Address = appUserDto.Address,
                Email = appUserDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(appUserDto.Password),
                Role = appUserDto.Role
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

            string token = GenerateToken(getUser, _config);

            return new Response(true, token);
        }

        public static string GenerateToken(AppUser user, IConfiguration config)
        {
            var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Name!),
                new(ClaimTypes.Email, user.Email!),
            };
            if(!string.IsNullOrEmpty(user.Role) || !Equals("string", user.Role))
                claims.Add(new(ClaimTypes.Role, user.Role!));

            var token = new JwtSecurityToken(
                issuer: config["Authentication:Issuer"],
                audience: config["Authentication:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
