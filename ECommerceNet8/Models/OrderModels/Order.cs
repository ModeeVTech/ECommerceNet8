using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceNet8.Models.OrderModels
{
    public class Order
    {
        public int OrderId { get; set; }
        public string OrderUniqueIdentifier { get; set; }
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }   
        public string? PhoneNumber { get; set; }
        public int? AppartmentNumber { get; set; }
        public int HouseNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }

        [Column(TypeName ="datetime")]
        public DateTime OrderTime { get; set; }
        public int OrderFromCustomerId { get; set; }
        public OrderFromCustomer OriginalOrderFromCustomer { get; set; }
        public ICollection<ItemAtCustomer> ItemsAtCustomer { get; set; }

    }
}
