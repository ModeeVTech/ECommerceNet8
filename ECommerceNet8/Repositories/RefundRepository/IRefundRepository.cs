using ECommerceNet8.DTOs.RefundRequestDtos.Request;
using ECommerceNet8.DTOs.RefundRequestDtos.Response;

namespace ECommerceNet8.Repositories.RefundRepository
{
    public interface IRefundRepository
    {
        public Task<Response_Refund> CreateRefundOrder(Request_Refund refundRequest);
        public Task<Response_RefundFullInfo> GetRefundRequest(string exchangeUniqueIdentifier);

        public Task<Response_RefundIsSuccess> AddReturnedGoodItem(Request_AddGoodRefundItem
            addGoodRefundItem);
        public Task<Response_RefundIsSuccess> CancelGoodReturnedItem(
            Request_CancelRefundItem cancelRefundItem);

        public Task<Response_RefundIsSuccess> AddReturnedBadItem
            (Request_AddBadRefundItem addBadRefundItem);
        public Task<Response_RefundIsSuccess> CancelBadReturnItem
            (Request_CancelRefundItem cancelRefundItem);

        public Task<Response_RefundIsSuccess> SetOrderAsRefunded(string exchangeUniqueIdentifier);
        public Task<Response_RefundIsSuccess> CancelOrderAsRefunded(string exchangeUniqueIdentifier);

        public Task<Response_RefundIsSuccess> SetOrderAsDone(string exchangeUniqueIdentifier);
        public Task<Response_RefundIsSuccess> CancelOrderAsDone(string exchangeUniqueIdentifier);

        public Task<Response_GoodRefundItems> GetAllGoodRefundItems
            (string exchangeUniqueIdentifier);
        public Task<Response_BadRefundItems> GetAllBadRefundItems
            (string exchangeUniqueIdentifier);
        public Task<Response_RefundIsSuccess> AllItemsCheckedSendEmail
            (string exchangeUniqueIdentifier);

    }
}
