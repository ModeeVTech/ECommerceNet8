using ECommerceNet8.DTOs.ProductVariantDtos.CustomModels;

namespace ECommerceNet8.DTOs.ProductVariantDtos.Response
{
    public class Response_ProductVariantWithObj
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public List<Model_ProductVariantReturn> ProductVariants { get; set; }
        = new List<Model_ProductVariantReturn>();
    }
}
