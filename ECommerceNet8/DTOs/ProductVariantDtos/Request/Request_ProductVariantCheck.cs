using ECommerceNet8.DTOs.ProductVariantDtos.CustomModels;

namespace ECommerceNet8.DTOs.ProductVariantDtos.Request
{
    public class Request_ProductVariantCheck
    {
        public IEnumerable<Model_ProductVariantCheckRequest> ProductVariants { get; set; }
    }
}
