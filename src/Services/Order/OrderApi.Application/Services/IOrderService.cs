using OrderApi.Application.Dtos;

namespace OrderApi.Application.Services
{
    public interface IOrderService
    {
        public Task<IEnumerable<OrderDto>> GetOrdersByClientId(int clientId);
        public Task<OrderDetailsDto> GetOrderDetails(int orderId);
    }
}
