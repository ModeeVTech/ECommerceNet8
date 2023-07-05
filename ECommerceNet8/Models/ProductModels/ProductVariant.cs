using System.Text.Json.Serialization;

namespace ECommerceNet8.Models.ProductModels
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public int BaseProductId { get; set; }
        [JsonIgnore]
        public BaseProduct baseProduct { get; set; }
        public int ProductColorId { get; set; }
        public ProductColor productColor { get; set; }
        public int ProductSizeId { get; set; }
        public ProductSize productSize { get; set; }
        public int Quantity { get; set; }
    }
}
