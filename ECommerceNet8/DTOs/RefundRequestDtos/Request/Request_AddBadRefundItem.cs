namespace ECommerceNet8.DTOs.RefundRequestDtos.Request
{
    public class Request_AddBadRefundItem
    {
        public string ExchangeUniqueIdentifier { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public string ReasonMessage { get; set; }
    }
}
