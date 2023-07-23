namespace ECommerceNet8.DTOs.AddressDtos.Request
{
    public class Request_AddressInfo
    {
        public int HouseNumber { get; set; }
        public int? ApparmentNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}
