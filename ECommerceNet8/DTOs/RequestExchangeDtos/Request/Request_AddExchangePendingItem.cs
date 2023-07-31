namespace ECommerceNet8.DTOs.RequestExchangeDtos.Request
{
    public class Request_AddExchangePendingItem
    {
        public string ExchangeUniqueIdentifier { get; set; }
        public int ReturnedProductVariantId { get; set; }
        public int Quantity { get; set; }
        public string Message { get; set; }
    }
}
