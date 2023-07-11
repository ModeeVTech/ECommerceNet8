namespace ECommerceNet8.DTOs.ProductVariantDtos.CustomModels
{
    public class Model_ProductVariantRequest
    {
        public int BaseProductId { get; set; }
        public int ProductColorId { get; set; }
        public int ProductSizeId { get; set; }
        public int Quantity { get; set; }
    }
}
