using OrderApi.Application.Dtos;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Mappings;
using Polly.Registry;
using System.Net.Http.Json;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrderRepository orderRepository,HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {
        //GET PRODUCT
        public async Task<ProductDto> GetProduct(int productId)
        {
            //call product api using http client
            //redirect this call to the Api Gateway and let the gateway handle the routing to the product api, as product api don't respond to outsiders

            var getProduct = await httpClient.GetAsync($"api/products/{productId}");
            if(!getProduct.IsSuccessStatusCode)
                return null!;
            var product = await getProduct.Content.ReadFromJsonAsync<ProductDto>();
            return product;

        }
        //GET USER

        public async Task<AppUserDto> GetUser(int userId)
        {
            //call product api using http client
            //redirect this call to the Api Gateway and let the gateway handle the routing to the product api, as product api don't respond to outsiders

            var getUser = await httpClient.GetAsync($"api/products/{userId}");
            if (!getUser.IsSuccessStatusCode)
                return null!;
            var user = await getUser.Content.ReadFromJsonAsync<AppUserDto>();
            return user;
        }

        //GET USER DETAILS BY ID
        public async Task<OrderDetailsDto> GetOrderDetails(int orderId)
        {
            var order = await orderRepository.FindByIdAsync(orderId);
            if (order is null)
                return null!;

            //Get Retry pipeline
            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");


            //Prepare Product
            //var productDto = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            //Prepare Client
            //var appUserDto = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            // OPTIMIZED:
            // Run API calls concurrently instead of sequentially

            var productTask = retryPipeline.ExecuteAsync(
                async token => await GetProduct(order.ProductId)
            ).AsTask();

            var userTask = retryPipeline.ExecuteAsync(
                async token => await GetUser(order.ClientId)
            ).AsTask();

            await Task.WhenAll(productTask, userTask);

            var productDto = await productTask;
            var appUserDto = await userTask;

            // ADDED:
            // Proper null handling

            if (productDto is null || appUserDto is null)
                return null;

            return new OrderDetailsDto(
                order.Id,
                productDto.Id,
                appUserDto.Id,
                appUserDto.UserName,  
                appUserDto.Email,
                appUserDto.Address,
                appUserDto.TelephoneNumber,
                productDto.Name,
                order.PurchaseQuantity,
                productDto.Price,
                productDto.Quantity * order.PurchaseQuantity,
                order.OrderedDate
                );
        }

        //GET ORDERS BY CLIENT ID
        public async Task<IEnumerable<OrderDto>> GetOrdersByClientId(int clientId)
        {
            //get all Client's orders

            var orders = await orderRepository.GetOrdersAsync(o => o.ClientId == clientId);

            if (!orders.Any()) return null!;

            return OrderMappings.ToDtoList(orders);
        }
    }
}
