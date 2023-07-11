using ECommerceNet8.DTOs.ProductVariantDtos.CustomModels;

namespace ECommerceNet8.DTOs.ProductVariantDtos.Response
{
    public class Response_ProductVariantWithoutObj
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public List<Model_ProductVariantWithoutObj> 
            ProductVariantWithoutObj { get; set; } = new List<Model_ProductVariantWithoutObj>();
    }
}
