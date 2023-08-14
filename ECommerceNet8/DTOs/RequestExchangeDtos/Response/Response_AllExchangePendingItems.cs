using ECommerceNet8.DTOs.RequestExchangeDtos.Models;

namespace ECommerceNet8.DTOs.RequestExchangeDtos.Response
{
    public class Response_AllExchangePendingItems
    {
        public bool isSuccess { get;set; }
        public string Message { get;set; }
        public string OrderUniqueIdentifier { get;set; }
        public string ExchangeUniqueIdentifier { get;set; }
        public List<ExchangeItemPendingDTO> ExchangeItemsPending { get; set; }
        = new List<ExchangeItemPendingDTO>();
    }
}
