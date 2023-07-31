using System.Text.Json.Serialization;

namespace ECommerceNet8.Models.OrderModels
{
    public class ExchangeConfirmedPdfInfo
    {
        public int Id { get; set; }
        public int ItemExchangeRequestId { get; set; }
        [JsonIgnore]
        public ItemExchangeRequest ItemExchangeRequest { get; set; }
        public string Name { get; set; }
        public DateTime Added { get; set; }
        public string Path { get; set; }
    }
}
