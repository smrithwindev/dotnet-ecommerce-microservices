using BuildingBlocks.Core.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Application.Mappings;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            //Get all products from the repo
            var products = await _productRepository.GetAllAsync();
            if (!products.Any())
            {
                return NotFound("No products found in the database");
            }
            //convert data from entity to DTOList
            var response = ProductMappings.ToDtoList(products);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            //Get single product from the repo
            var product = await _productRepository.FindByIdAsync(id);
            if (product == null)
            {
                return NotFound($"product with ID {id} is not found");
            }
            //convert data from entity to DTO
            var response = ProductMappings.ToDto(product);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(ProductCreationDto productDto)
        {
            //Check ModelState if all annotations are passed
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //convert data to entity
            var product = ProductMappings.ToEntity(productDto);
            //Add product to the repo
            var response = await _productRepository.CreateAsync(product);
            return response.flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDto productDto)
        {
            //Check ModelState if all annotations are passed
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //convert data to entity
            var product = ProductMappings.ToEntity(productDto);
            //Update product in the repo
            var response = await _productRepository.UpdateAsync(product);
            return response.flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response>> DeleteProduct(int id)
        {
            var response = await _productRepository.DeleteAsync(id);
            return response.flag ? Ok(response) : NotFound(response);
        }
    }
}
