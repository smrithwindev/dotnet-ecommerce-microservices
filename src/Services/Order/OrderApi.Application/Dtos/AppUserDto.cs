using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.Dtos
{
    public record AppUserDto(
        int Id,
        [Required] string UserName, //or simply name, as the username is the name of the client
        [Required] string TelephoneNumber,
        [Required] string Address,
        [Required, EmailAddress] string Email,
        [Required] string Password,
        [Required] string Role
    );

}
