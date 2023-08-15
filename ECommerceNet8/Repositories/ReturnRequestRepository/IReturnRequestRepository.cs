using ECommerceNet8.DTOs.ReturnDtos.Request;
using ECommerceNet8.DTOs.ReturnDtos.Response;
using ECommerceNet8.Models.ReturnExchangeModels;

namespace ECommerceNet8.Repositories.ReturnRequestRepository
{
    public interface IReturnRequestRepository
    {
        public Task<ICollection<ReturnRequestFromUser>> GetAllReturnRequest();

        public Task<ICollection<ReturnRequestFromUser>> GetReturnRequestByOrderUniqueId
            (string orderUniqueIdentifier);

        public Task<ReturnRequestFromUser> GetReturnRequestByExchangeUniqueId
            (string exchangeUniqueIdentifier);

        public Task<Response_ReturnRequest> AddReturnRequest
            (Request_ReturnRequest returnRequest, string userId);
    }
}
