using ECommerceNet8.Models.OrderModels;

namespace ECommerceNet8.DTOs.RequestExchangeDtos.Response
{
    public class Response_ReturnedItems
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<ReturnedItemsFromCustomer> returnedItemsFromCustomers { get; set; }
        = new List<ReturnedItemsFromCustomer>();
    }
}
