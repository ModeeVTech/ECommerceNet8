using ECommerceNet8.Models.ProductModels;

namespace ECommerceNet8.DTOs.ProducSizeDtos.Response
{
    public class Response_ProductSize
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }

        public List<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
    }
}
