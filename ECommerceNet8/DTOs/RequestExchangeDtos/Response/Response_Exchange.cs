namespace ECommerceNet8.DTOs.RequestExchangeDtos.Response
{
    public class Response_Exchange
    {
        public bool isSuccess { get; set; }
        public int ExchangeRequestId { get; set; }
        public string OrderUniqueIdentifier { get; set; }
        public string ExchangeUniqueIdentfier { get; set; }
        public string Message { get; set; }
    }
}
