using ECommerceNet8.DTOs.ProductColorDtos.Request;
using ECommerceNet8.DTOs.ProductColorDtos.Response;

namespace ECommerceNet8.Repositories.ProductColorRepository
{
    public interface IProductColorRepository
    {
        Task<Response_ProductColor> GetAllProductColors();
        Task<Response_ProductColor> GetProductColorById(int productColorId);
        Task<Response_ProductColor> AddProductColor(Request_ProductColor productColor);
        Task<Response_ProductColor> UpdateProductColor(int productColorId,
            Request_ProductColor productColor);
        Task<Response_ProductColor> DeleteProductColor(int productColorId);
    }
}
