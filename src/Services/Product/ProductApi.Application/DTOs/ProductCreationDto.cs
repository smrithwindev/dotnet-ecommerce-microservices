using System.ComponentModel.DataAnnotations;

namespace ProductApi.Application.DTOs
{
    public record ProductCreationDto
    (
         [Required] string Name,
         [Required, DataType(DataType.Currency)] decimal Price,
         [Required, Range(1,int.MaxValue)]int Quantity
    );
}
