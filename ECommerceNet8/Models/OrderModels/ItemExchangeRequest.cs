using System.Text.Json.Serialization;

namespace ECommerceNet8.Models.OrderModels
{
    public class ItemExchangeRequest
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [JsonIgnore]
        public Order Order { get; set; }
        public string OrderUniqueIdentifier { get; set; }
        public string ExchangeUniqueIdentifier { get; set; }

        public string AdminId { get; set; }
        public string AdminName { get; set; }
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

        public ICollection<ExchangeOrderItem> exchangeOrderItems { get; set; }
        public ICollection<ExchangeItemPending> exchangeItemsPending { get; set; }
        public ICollection<ExchangeItemCanceled> exchangeItemsCanceled { get; set; }

        public int ExchangeConfirmedPdfInfoId { get; set; }
        public ExchangeConfirmedPdfInfo exchangeConfirmedPdfInfo { get; set; }

        public bool RequestClosed { get; set; }

    }
}
