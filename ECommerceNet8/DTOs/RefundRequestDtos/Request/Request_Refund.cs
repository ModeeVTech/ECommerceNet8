namespace ECommerceNet8.DTOs.RefundRequestDtos.Request
{
    public class Request_Refund
    {
        public string OrderUniqueIdentifer { get; set; }
        public string ExchangeUniqueIdentifier { get; set; }
        public DateTime ExchangeRequestTime { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }

        public string AdminId { get; set; }
        public string AdminName { get; set; }
    }
}
