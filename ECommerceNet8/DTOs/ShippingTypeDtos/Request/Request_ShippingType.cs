using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceNet8.DTOs.ShippingTypeDtos.Request
{
    public class Request_ShippingType
    {
        public string ShippingFirmName { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public decimal Price { get; set; }
        public bool FreeTier { get; set; }
    }
}
