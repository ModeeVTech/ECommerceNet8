namespace ECommerceNet8.DTOs.RequestExchangeDtos.Request
{
    public class Request_ExchangeByAdmin
    {
        public string OrderUniqueIdentifier { get; set; }
        public string AdminId { get; set; }
        public string AdminFullName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public int? ApartmentNumber { get; set; }
        public int HouseNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

    }
}
