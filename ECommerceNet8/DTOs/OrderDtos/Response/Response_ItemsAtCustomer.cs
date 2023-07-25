using ECommerceNet8.Models.OrderModels;

namespace ECommerceNet8.DTOs.OrderDtos.Response
{
    public class Response_ItemsAtCustomer
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public int OrderId { get; set; }
        public string OrderUniqueIdentifier { get; set; }
        public IEnumerable<ItemAtCustomer>ItemsAtCustomer { get; set; }
    }
}
