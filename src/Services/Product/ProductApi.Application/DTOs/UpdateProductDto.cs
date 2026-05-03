using System.ComponentModel.DataAnnotations;

namespace ProductApi.Application.DTOs
{
    public record UpdateProductDto
    (
         [Required] string Name,
         [RequiredAttribute, DataType(DataType.Currency)] decimal Price,
         [Required, Range(1,int.MaxValue)]int Quantity
    );
}
