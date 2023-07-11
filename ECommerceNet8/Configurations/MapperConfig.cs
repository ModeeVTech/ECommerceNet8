using AutoMapper;
using ECommerceNet8.DTOs.MainCategoryDtos.Request;
using ECommerceNet8.DTOs.ProducSizeDtos.Request;
using ECommerceNet8.DTOs.ProductColorDtos.Request;
using ECommerceNet8.DTOs.ProductDtos.Request;
using ECommerceNet8.DTOs.ProductVariantDtos.CustomModels;
using ECommerceNet8.Models.ProductModels;

namespace ECommerceNet8.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Request_ProductMaterial, Material>().ReverseMap();
            CreateMap<Request_MainCategory, MainCategory>().ReverseMap();
            CreateMap<Request_ProductColor, ProductColor>().ReverseMap();
            CreateMap<Request_ProductSize, ProductSize>().ReverseMap();
            CreateMap<ProductVariant, Model_ProductVariantReturn>().ReverseMap();
            CreateMap<ProductVariant, Model_ProductVariantRequest>().ReverseMap();
        }
    }
}
