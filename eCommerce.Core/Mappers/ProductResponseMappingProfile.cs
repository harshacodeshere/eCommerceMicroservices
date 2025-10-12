using AutoMapper;
using eCommerce.Core.DTO;
using eCommerce.Core.Entities;
using Microsoft.AspNetCore.Http;

namespace eCommerce.Core.Mappers;

public class ProductResponseMappingProfile : Profile
{
    public ProductResponseMappingProfile()
    {
        CreateMap<Product, ProductResponse>()
    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
    .ForMember(dest => dest.QuantityInStock, opt => opt.MapFrom(src => src.QuantityInStock))
    .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
    .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
    .ForMember(dest => dest.ProductId, opt => opt.Ignore());
    }
}
