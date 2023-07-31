using ECommerceNet8.Models.OrderModels;

namespace ECommerceNet8.DTOs.RequestExchangeDtos.Response
{
    public class Response_ExchangeFullInfo
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int OrderId { get; set; }
        public string OrderUniqueIdentifier { get; set; }
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

        public List<ExchangeOrderItem> ExchangeOrderItems { get; set; } 
            = new List<ExchangeOrderItem>();

        public List<ExchangeItemPending> exchangeItemsPending { get; set; }
        = new List<ExchangeItemPending>();

        public List<ExchangeItemCanceled> ExchangeItemsCanceled { get; set; } 
            = new List<ExchangeItemCanceled>();

        public int exchangeConfirmedPdfInfoId { get; set; } 
        public ExchangeConfirmedPdfInfo ExchangeConfirmedPdfInfo { get; set; }

        public bool RequestClosed { get; set; }
    }
}
