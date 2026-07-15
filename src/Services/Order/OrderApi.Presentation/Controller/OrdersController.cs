using BuildingBlocks.Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.Dtos;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Mappings;
using OrderApi.Application.Services;
using OrderApi.Infrastructure.Repositories;

namespace OrderApi.Presentation.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderService _orderService;

        public OrdersController(IOrderRepository orderRepository, IOrderService orderService)
        {
            _orderRepository = orderRepository;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var orders = await _orderRepository.GetAllAsync();
            if (!orders.Any())
                return NotFound("No orders found in the database");
            var response = OrderMappings.ToDtoList(orders);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _orderRepository.FindByIdAsync(id);
            if (order is null)
                return NotFound($"Order with ID {id} not found");
            var response = OrderMappings.ToDto(order);
            return Ok(response);
        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetClientOrders(int clientId)
        {
            if (clientId <= 0)
                return BadRequest("Invalid client id");
            var orders = await _orderService.GetOrdersByClientId(clientId);
            if (!orders.Any())
                return NotFound("No orders found for this client");
            return Ok(orders);
        }

        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDetailsDto>> GetOrderDetails(int orderId)
        {
            if (orderId <= 0)
                return BadRequest("Invalid order id");
            var orderDetail = await _orderService.GetOrderDetails(orderId);
            if (orderDetail is null)
                return NotFound("No order found");
            return Ok(orderDetail);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(CreateOrderDto createOrderDto)
        {
            //if (!ModelState.IsValid)
            //  return BadRequest(ModelState);
            var order = OrderMappings.ToEntity(createOrderDto);
            var response = await _orderRepository.CreateAsync(order);
            return response.flag ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(UpdateOrderDto updateOrderDto)
        {
            //if (!ModelState.IsValid)    if my api already contains [ApiController] no need to have "ModelState" as ASP.NET Core already performs automatic validation.
              //  return BadRequest(ModelState);
            var order = OrderMappings.ToEntity(updateOrderDto);
            var response = await _orderRepository.UpdateAsync(order);
            return response.flag ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response>> DeleteOrder(int id)
        {
            var response = await _orderRepository.DeleteAsync(id);
            return response.flag ? Ok(response) : NotFound(response);
        }
    }
}
