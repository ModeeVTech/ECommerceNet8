using ECommerceNet8.DTOs.ShippingTypeDtos.Request;
using ECommerceNet8.DTOs.ShippingTypeDtos.Response;
using ECommerceNet8.Models.OrderModels;

namespace ECommerceNet8.Repositories.ShippingTypeRepository
{
    public interface IShippingTypeRepository
    {
        Task<IEnumerable<ShippingType>> GetShippingTypes();
        Task<ShippingType> GetShippingTypeById(int shippingTypeId);
        Task<ShippingType> AddShippingType(Request_ShippingType shippingType);
        Task<Response_ShippingType> UpdateShippingType(int shippingTypeId,
            Request_ShippingType shippingType);
        Task<Response_ShippingType> DeleteShippingType(int shippingTypeId);
    }
}
