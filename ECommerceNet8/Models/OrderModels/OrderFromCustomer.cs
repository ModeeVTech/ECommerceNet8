using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ECommerceNet8.Models.OrderModels
{
    public class OrderFromCustomer
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [JsonIgnore]
        public Order Order { get; set; }

        public  ICollection<OrderItem> OrderItems { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Discount { get; set; }

        public int? ShippingTypeId { get; set; }
        public string? ShippingFirmName { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ShippingPrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalPrice { get; set; }
        public int PdfInfoId { get; set; }
        public PdfInfo pdfInfo { get; set; }

        public bool OrderCanceled { get; set; }
        public bool RefundMade { get; set; }
        public bool ItemSent { get; set; }
    }
}
