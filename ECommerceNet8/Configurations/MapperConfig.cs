using AutoMapper;
using ECommerceNet8.DTOs.ProductDtos.Request;
using ECommerceNet8.Models.ProductModels;

namespace ECommerceNet8.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Request_ProductMaterial, Material>().ReverseMap();
        }
    }
}
