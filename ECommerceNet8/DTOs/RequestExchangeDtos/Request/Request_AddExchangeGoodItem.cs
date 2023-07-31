namespace ECommerceNet8.DTOs.RequestExchangeDtos.Request
{
    public class Request_AddExchangeGoodItem
    {
        public string OrderUniqueIdentifier { get; set; }
        public string ExchangeUniqueIdentifier { get; set; }
        public int ReturnedProductVariantId { get; set; }
        public int ExchangeProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}
