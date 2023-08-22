using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ECommerceNet8.Models.OrderModels
{
    public class ItemBadForRefund
    {
        public int Id { get; set; }
        public int ItemReturnRequestId { get; set; }
        [JsonIgnore]
        public ItemReturnRequest itemReturnRequest { get; set; }

        public int BaseProductId { get; set; }
        public string BaseProductName { get; set; }
        public int ProductVariantId { get; set; }
        public string ProductColor { get; set; }
        public string ProductSize { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal PricePaidPerItem { get; set; }
        public int Quantity { get; set; }
        public string ReasonForNotRefunding { get; set; }
    }
}
