using System.ComponentModel.DataAnnotations;

namespace ECommerceNet8.DTOs.BaseProductDtos.Request
{
    public class Request_BaseProductDiscount
    {
        [Range(0,99)]
        public int Discount { get; set; }
    }
}
