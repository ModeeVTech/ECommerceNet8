using ECommerceNet8.Models.ProductModels;

namespace ECommerceNet8.DTOs.ProductColorDtos.Response
{
    public class Response_ProductColor
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }

        public List<ProductColor> productColors { get; set; } = new List<ProductColor>();
    }
}
