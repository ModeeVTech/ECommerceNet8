using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceNet8.Models.OrderModels
{
    public class ShippingType
    {
        public int ShippingTypeId { get; set; }
        public string ShippingFirmName { get; set; }
        [Column(TypeName =("decimal(18,2)"))]
        public decimal Price { get; set; }
        public bool FreeTier { get; set; }
    }
}
