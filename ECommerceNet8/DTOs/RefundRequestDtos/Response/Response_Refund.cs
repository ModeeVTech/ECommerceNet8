namespace ECommerceNet8.DTOs.RefundRequestDtos.Response
{
    public class Response_Refund
    {
        public bool isSuccess { get; set; }
        public string OrderUniqueIdentifier { get; set; }
        public string ExchangeUniqueIdentifier { get; set; }
        public int ReturnRequestId { get; set; }
        public string Message { get; set; }
    }
}
