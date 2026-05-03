using ProductApi.Application.DTOs;
using ProductApi.Domain.Entities;
using System.Data;     //here we are not using AutoMapper for simplicity, we are doing manual mapping

namespace ProductApi.Application.Mappings
{
        public static class ProductMappings
        {
            // 🔹 Single entity → DTO
            public static ProductDto ToDto(Product product)
            {
                return new ProductDto
                (
                    Id: product.Id,
                    Name: product.Name!,
                    Quantity: product.Quantity,
                    Price: product.Price
                );
            }

            // 🔹 Collection → DTO list
            public static IEnumerable<ProductDto> ToDtoList(IEnumerable<Product> products)
            {
                return products.Select(p => new ProductDto
                (
                    p.Id,
                    p.Name!,
                    p.Quantity,
                    p.Price
                ));
            }

            // 🔹 DTO → Entity
            public static Product ToEntity(ProductDto dto)
            {
                return new Product
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Quantity = dto.Quantity,
                    Price = dto.Price
                };
            }

            public static Product ToEntity(UpdateProductDto dto)
            {
                return new Product
                {
                    Name = dto.Name,
                    Quantity = dto.Quantity,
                    Price = dto.Price
                };
        }

        //when you only require bulk operations
        public static IEnumerable<Product> ToEntityList(IEnumerable<ProductDto> dtos)
            {
                return dtos.Select(dto => new Product
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Quantity = dto.Quantity,
                    Price = dto.Price
                });
            }
    }
}