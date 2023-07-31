using ECommerceNet8.DTOs.RequestExchangeDtos.Request;
using ECommerceNet8.DTOs.RequestExchangeDtos.Response;
using ECommerceNet8.Models.ReturnExchangeModels;

namespace ECommerceNet8.Repositories.ReturnExchangeRequestRepository
{
    public interface IReturnExchangeRequestRepository
    {

        //FOR USERS
        public Task<ICollection<ExchangeRequestFromUser>> GetExchangeRequestFromUsers();
        public Task<ICollection<ExchangeRequestFromUser>> 
            GetExchangeRequestByOrderUniqueIdentifier(string orderUniqueIdentifier);
        public Task<ExchangeRequestFromUser> GetExchangeRequestByExchangeUniqueId
            (string exchangeUniqueIdentifier);
        public Task<Response_ExchangeRequest> AddExchangeRequest(Request_ExchangeRequest exchangeRequest,
            string UserId);



        public Task<Response_Exchange> CreateExchangeRequest(Request_Exchange exchangeRequest);
        public Task<Response_Exchange> CreateExchangeRequestByAdmin(Request_ExchangeByAdmin exchangeRequest);

        public Task<Response_ExchangeFullInfo> GetExchangeRequest(
            string exchangeUniqueIdentifier);

        public Task<Response_IsSuccess> MarkExchangeOrderAsDone(string exchangeUniqueIdentifier);
        public Task<Response_IsSuccess> MarkExchangeOrderAsNotDone(string exchangeUniqueIdentifier);
        public Task<Response_Exchange> SendEmailWithPendingInfo(string exchangeUniqueIdentifier);
        public Task<Response_Exchange> SendEmailWithCompletedPdf(string exchangeUniqueIdentifier);
        public Task<Response_AllExchangedGoodItems> GetAllExchangeGoodItems(string exchangeUniqueIdentifier);

        public Task<Response_IsSuccess> AddExchangeGoodItem(Request_AddExchangeGoodItem exchangeGoodItem);

        public Task<Response_IsSuccess> RemoveExchangeGoodItem(Request_RemoveExchangeGoodItem exchangeGoodItem);

        public Task<Response_AllExchangePendingItems> GetAllExchangePendingItems(
            string exchangeUniqueIdentifier);
        public Task<Response_IsSuccess> AddExchangePendingItem(
            Request_AddExchangePendingItem exchangePendingItem);
        public Task<Response_IsSuccess> RemoveExchangePendingItem(
            Request_RemoveExchangePendingItem exchangePendingItem);
        public Task<Response_IsSuccess> MovePendingItemToGood
            (Request_MovePendingToGood movePendingToGood);
        public Task<Response_IsSuccess> MovePendingItemToBad
            (Request_MovePendingToBad movePendingToBad);

        public Task<Response_AllExchangeBadItems> GetAllExchangeBadItems
            (string exchangeUniqueIdentifier);
        public Task<Response_IsSuccess> AddExchangeBadItem(
            Request_AddExchangeBadItem addExchangeBadItem);
        public Task<Response_IsSuccess> RemoveExchangeBadItem
            (Request_RemoveExchangeBadItem removeExchangeBadItem);

        //FOR RETURNED ITEMS
        public Task<Response_IsSuccess> AddItemToReturn(int itemAtCustomerId, int quantity);
        public Task<Response_IsSuccess> RemoveItemFromReturn(int returnedItemId, int quantity);
        public Task<Response_ReturnedItems> GetAllReturnedItems(int orderId);

    }
}
