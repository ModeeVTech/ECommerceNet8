namespace ECommerceNet8.DTOs.ReturnDtos.Request
{
    public class Request_ReturnRequest
    {
        public string OrderUniqueIdentifier { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string Message { get; set; }
    }
}
