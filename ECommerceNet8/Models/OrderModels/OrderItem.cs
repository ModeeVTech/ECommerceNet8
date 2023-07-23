using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ECommerceNet8.Models.OrderModels
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderFromCustomerId { get; set; }
        [JsonIgnore]
        public OrderFromCustomer orderFromCustomer { get; set; }
        public int BaseProductId { get; set; }
        public string BaseProductName { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductVariantColor { get; set; }
        public string ProductVariantSize { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName ="decimal(18,2)")]
        public decimal Price { get; set; }
        public int Discount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PricePerItem { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
    }
}
