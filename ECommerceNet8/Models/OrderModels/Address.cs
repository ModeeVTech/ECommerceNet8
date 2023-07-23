namespace ECommerceNet8.Models.OrderModels
{
    public class Address
    {
        public int AddressId { get; set; }
        public string UserId { get; set; }
        public int HouseNumber { get; set; }
        public int? AppartmentNumber { get; set; }  
        public string Street { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }
}
