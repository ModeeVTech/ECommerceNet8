using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ECommerceNet8.Models.OrderModels
{
    public class ItemAtCustomer
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [JsonIgnore]
        public Order Order { get; set; }
        public int BaseProductId { get; set; }
        public string BaseProductName { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductVariantColor { get; set; }
        public string ProductVariantSize { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal PricePaidPerItem { get; set; }
        public int Quantity { get; set; }
    }
}
