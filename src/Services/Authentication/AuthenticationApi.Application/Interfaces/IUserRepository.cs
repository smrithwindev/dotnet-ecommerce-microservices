using AuthenticationApi.Application.Dtos;
using AuthenticationApi.Domain.Entities;
using BuildingBlocks.Core.Responses;


namespace AuthenticationApi.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser> GetUserByEmail(string email);
        Task<GetUserDto> GetUser(int userId);
        Task<Response> Register(AppUserDto appUserDto);
        Task<Response> Login(LoginDto loginDto);
    }
}
