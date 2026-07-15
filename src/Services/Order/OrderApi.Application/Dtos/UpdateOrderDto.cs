using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.Dtos
{
    public record UpdateOrderDto(
    [Range(1, int.MaxValue)] int Id,
    [Range(1, int.MaxValue)] int ProductId,
    [Range(1, int.MaxValue)] int ClientId,
    [Range(1, int.MaxValue)] int PurchaseQuantity
);
}
