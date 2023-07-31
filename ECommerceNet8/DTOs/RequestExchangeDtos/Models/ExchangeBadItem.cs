using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceNet8.DTOs.RequestExchangeDtos.Models
{
    public class ExchangeBadItem
    {
        public int Id { get; set; }
        public int BaseProductId { get; set; }
        public string BaseProductName { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal PricePerItemPaid { get; set; }
        public int ReturnedProductVariantId { get; set; }
        public string ReturnedProductVariantColor { get; set; }
        public string ReturnedProductVariantSize { get; set; }
        public int Quantity { get; set; }
        public string CanceliationReason { get; set; }
    }
}
