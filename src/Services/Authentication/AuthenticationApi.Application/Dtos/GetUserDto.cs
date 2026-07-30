using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.Dtos
{
    public record GetUserDto(
        int Id,
        [Required] string UserName, 
        [Required] string TelephoneNumber,
        [Required] string Address,
        [Required, EmailAddress] string Email,
        [Required] string Role
    );
}
