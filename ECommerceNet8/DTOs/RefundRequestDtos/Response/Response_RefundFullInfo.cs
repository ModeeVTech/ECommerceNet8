using ECommerceNet8.Models.OrderModels;

namespace ECommerceNet8.DTOs.RefundRequestDtos.Response
{
    public class Response_RefundFullInfo
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderUniqueIdentifier { get; set; }
        public string ExchangeUniqueIdentifier { get; set; }
        public string AdminId { get; set; }
        public string AdminFullName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string UserBankName { get; set; }
        public string UserBankAccount { get; set; }
        public DateTime ExchangeRequestTime { get; set; }


        public ICollection<ItemGoodForRefund> ItemsGoodForRefund { get; set; }
        public ICollection<ItemBadForRefund>  ItemsBadForRefund { get; set;}

        public decimal? TotalRequestForRefund { get; set; }
        public decimal? TotalAmountNotRefunded { get; set; }
        public decimal? TotalAmountRefunded { get; set; }

        public bool RequestRefunded { get; set; }
        public bool RequestClosed { get; set; }
    }
}
