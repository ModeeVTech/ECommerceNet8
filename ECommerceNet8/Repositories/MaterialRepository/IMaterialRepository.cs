using ECommerceNet8.DTOs.ProductDtos.Request;
using ECommerceNet8.DTOs.ProductDtos.Response;

namespace ECommerceNet8.Repositories.MaterialRepository
{
    public interface IMaterialRepository
    {
        Task<Response_ProductMaterial> GetAllProductMaterials();
        Task<Response_ProductMaterial> GetProductMaterialById(int productMaterialId);
        Task<Response_ProductMaterial> AddProductMaterial(Request_ProductMaterial productMaterialDto);
        Task<Response_ProductMaterial> UpdateProductMaterial(int productMaterialId,
            Request_ProductMaterial productMaterialDto);
        Task<Response_ProductMaterial> DeleteProductMaterial(int productMaterialId);
    }
}
