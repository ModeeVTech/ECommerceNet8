using ECommerceNet8.Models.ProductModels;

namespace ECommerceNet8.DTOs.MainCategoryDtos.Response
{
    public class Response_MainCategory
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public List<MainCategory> mainCategories { get; set; } = new List<MainCategory>();
    }
}
