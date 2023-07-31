using ECommerceNet8.DTOs.RequestExchangeDtos.Models;

namespace ECommerceNet8.DTOs.RequestExchangeDtos.Response
{
    public class Response_AllExchangedGoodItems
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string OrderUniqueIdentifier { get; set; }
        public string ExchangeUniqueIdentfier { get; set; }
        public List<ExchangeGoodItem> exchangeGoodItems { get; set; } =
            new List<ExchangeGoodItem>();
    }
}
