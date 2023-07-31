namespace ECommerceNet8.DTOs.RequestExchangeDtos.Request
{
    public class Request_MovePendingToBad
    {
        public int PendingItemId { get; set; }
        public int Quantity { get; set; }
        public string Message { get; set; }
    }
}
