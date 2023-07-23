using System.Text.Json.Serialization;

namespace ECommerceNet8.Models.OrderModels
{
    public class PdfInfo
    {
        public int Id { get; set; }
        public int OrderFromCustomerId { get; set; }
        [JsonIgnore]
        public OrderFromCustomer OrderFromCustomer { get; set; }
        public string Name { get; set; }
        public DateTime Added { get; set; }
        public string Path { get; set; }
    }
}
