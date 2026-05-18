using OrderApi.Application.Dtos;
using OrderApi.Domain.Entities;

namespace OrderApi.Application.Mappings
{
    public static class OrderMappings
    {
        // Single Entity -> DTO
        public static OrderDto ToDto(Order order)
        {
            return new OrderDto
            (
                order.Id,
                order.ProductId,
                order.ClientId,
                order.PurchaseQuantity,
                order.OrderedDate
            );
        }

        // Collection -> DTO List
        public static IEnumerable<OrderDto> ToDtoList(IEnumerable<Order> orders)
        {
            return orders.Select(ToDto);
        }

        // DTO -> Entity
        public static Order ToEntity(OrderDto dto)
        {
            return new Order
            (
                dto.ProductId,
                dto.ClientId,
                dto.PurchaseQuantity
            );
        }

        // Bulk DTO -> Entity
        public static IEnumerable<Order> ToEntityList(IEnumerable<OrderDto> dtos)
        {
            return dtos.Select(ToEntity);
        }
    }
}