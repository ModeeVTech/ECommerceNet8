using ECommerceNet8.DTOs.MainCategoryDtos.Request;
using ECommerceNet8.DTOs.MainCategoryDtos.Response;

namespace ECommerceNet8.Repositories.MainCategoryRepository
{
    public interface IMainCategoryRepository
    {
        Task<Response_MainCategory> GetAllMainCategories();
        Task<Response_MainCategory> GetMainCategoryById(int mainCategoryId);
        Task<Response_MainCategory> AddMainCategory(Request_MainCategory mainCategory);
        Task<Response_MainCategory> UpdateMainCategory(int mainCategoryId,
            Request_MainCategory mainCategory);
        Task<Response_MainCategory> DeleteMainCategory(int mainCategoryId);
    }
}
