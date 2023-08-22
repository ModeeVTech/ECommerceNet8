namespace ECommerceNet8.DTOs.RefundRequestDtos.Request
{
    public class Request_AddGoodRefundItem
    {
        public string ExchangeUniqueIdentifier { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}
