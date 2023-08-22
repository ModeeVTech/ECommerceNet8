using ECommerceNet8.Models.OrderModels;

namespace ECommerceNet8.DTOs.RefundRequestDtos.Response
{
    public class Response_GoodRefundItems
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public string OrderUniqueIdentifier { get; set; }
        public string ExchangeUniqueIdentifier { get; set; }
        public List<ItemGoodForRefund> itemsGoodForRefund { get; set; }
        = new List<ItemGoodForRefund>();
    }
}
