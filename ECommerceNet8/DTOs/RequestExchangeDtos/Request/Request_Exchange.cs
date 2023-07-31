namespace ECommerceNet8.DTOs.RequestExchangeDtos.Request
{
    public class Request_Exchange
    {
        public string OrderUniqueIdentfier { get; set; }
        public string ExchangeUniqueIdentfier { get; set; }

        public string AdminId { get; set; }
        public string AdminFullName { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
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
