namespace ECommerceNet8.DTOs.RequestExchangeDtos.Request
{
    public class Request_ExchangeRequest
    {
        public string OrderUniqueIdentifier { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int? ApartmentNumber { get; set; }
        public int HouseNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Message { get; set; }
    }
}
