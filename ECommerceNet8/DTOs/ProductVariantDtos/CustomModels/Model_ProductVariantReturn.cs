using ECommerceNet8.Models.ProductModels;

namespace ECommerceNet8.DTOs.ProductVariantDtos.CustomModels
{
    public class Model_ProductVariantReturn
    {
        public int Id { get; set; }
        public int BaseProductId { get; set; }
        public ProductColor productColor { get; set; }
        public ProductSize productSize { get; set; }
        public int Quantity { get; set; }   
    }
}
