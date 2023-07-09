using ECommerceNet8.DTOs.BaseProductDtos.CustomModels;
using ECommerceNet8.DTOs.BaseProductDtos.Request;
using ECommerceNet8.DTOs.BaseProductDtos.Response;
using ECommerceNet8.Models.ProductModels;

namespace ECommerceNet8.Repositories.BaseProductRepository
{
    public interface IBaseProductRepository
    {
        Task<IEnumerable<BaseProduct>> GetAllAsync();
        Task<IEnumerable<Model_BaseProductCustom>> GetAllWithFullInfoAsync();
        Task<Response_BaseProductWithPaging> GetAllWithFullInfoByPages(
            int pageNumber, int pageSize);
        Task<Response_BaseProduct> GetByIdWithNoInfo(int baseProductId);
        Task<Response_BaseProductWithFullInfo> GetByIdWithFullInfo(int baseProductId);


        Task<Response_BaseProduct> AddBaseProduct(Request_BaseProduct baseProduct);
        Task<Response_BaseProduct>  UpdateBaseProduct(int baseProductId,
            Request_BaseProduct baseProduct);
        Task<Response_BaseProduct> UpdateBaseProductPrice(int baseProductId,
            Request_BaseProductPrice baseProductPrice);
        Task<Response_BaseProduct> UpdateBaseProductDiscount(int baseProductId,
            Request_BaseProductDiscount baseProductDiscount);
        Task<Response_BaseProduct> UpdateBaseProductMainCategory(int baseProductId,
            Request_BaseProductMainCategory baseProductMainCategory);
        Task<Response_BaseProduct> UpdateBaseProductMaterial(int baseProductId,
            Request_BaseProductMaterial baseProductMaterial);

        Task<Response_BaseProduct> RemoveBaseProduct(int baseProductId);

        //SEARCH
        Task<IEnumerable<string>> GetProductSearchSuggestions(string searchText);
        Task<IEnumerable<Model_BaseProductCustom>> GetProductSearch(string searchText);
        Task<Response_BaseProductWithPaging> GetProductSearchWithPaging(string searchText,
            int pageNumber, int pageSize);

        Task<IEnumerable<Model_BaseProductCustom>> SearchProducts(
            int[] MaterialsIds, int[] mainCategoryIds, int[] productColorIds,
            int[] productSizeIds);

    }
}
