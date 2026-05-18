using ProductApi.Application.DTOs;
using ProductApi.Domain.Entities;
using System.Data;     //here we are not using AutoMapper for simplicity, we are doing manual mapping

namespace ProductApi.Application.Mappings
{
        public static class ProductMappings
        {
            //Single entity → DTO
            public static ProductDto ToDto(Product product)
            {
                return new ProductDto
                (
                    product.Id,
                    product.Name!,
                    product.Quantity,
                    product.Price
                );
            }

            //Collection → DTO list
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

             //Other way of writing the above method using method group conversion

        //public static IEnumerable<ProductDto> ToDtoList(IEnumerable<Product> products) => products.Select(ToDto);

        //DTO → Entity
        public static Product ToEntity(ProductDto dto)
            {
                return new Product
                (
                    //Id = dto.Id,
                    dto.Name,
                    dto.Price,
                    dto.Quantity
                );
            }

            public static Product ToEntity(ProductCreationDto dto)
            {
                return new Product
                (
                    dto.Name,
                    dto.Price,
                    dto.Quantity
                );
        }

        //when you only require bulk operations

        public static IEnumerable<Product> ToEntityList(IEnumerable<ProductDto> dtos) => dtos.Select(ToEntity);

        /*public static IEnumerable<Product> ToEntityList(IEnumerable<ProductDto> dtos)
            {
                return dtos.Select(dto => new Product
                (
                    dto.Name,
                    dto.Price,
                    dto.Quantity
                ));
            }
        */


    }
}