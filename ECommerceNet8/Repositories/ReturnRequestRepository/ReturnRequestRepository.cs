using ECommerceNet8.Data;
using ECommerceNet8.DTOs.ReturnDtos.Request;
using ECommerceNet8.DTOs.ReturnDtos.Response;
using ECommerceNet8.Models.ReturnExchangeModels;
using Microsoft.EntityFrameworkCore;

namespace ECommerceNet8.Repositories.ReturnRequestRepository
{
    public class ReturnRequestRepository : IReturnRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public ReturnRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<ICollection<ReturnRequestFromUser>> GetAllReturnRequest()
        {
            var existingReturnRequest = await _db.returnRequestsFromUsers.ToListAsync();

            return existingReturnRequest;
        }
        public async Task<ICollection<ReturnRequestFromUser>> GetReturnRequestByOrderUniqueId(string orderUniqueIdentifier)
        {
            var existingReturnRequests = await _db.returnRequestsFromUsers
                .Where(rr=>rr.OrderUniqueIdentifier == orderUniqueIdentifier)
                .ToListAsync();
            
            return existingReturnRequests;
        }

        public async Task<ReturnRequestFromUser> GetReturnRequestByExchangeUniqueId(string exchangeUniqueIdentifier)
        {
            var existingReturnRequest = await _db.returnRequestsFromUsers
                .FirstOrDefaultAsync(rr=>rr.ExchangeUniqueIdentifier == exchangeUniqueIdentifier);

            return existingReturnRequest;
        }

        public async Task<Response_ReturnRequest> AddReturnRequest(Request_ReturnRequest returnRequest, string userId)
        {
            var existingOrder = await _db.Orders
            .FirstOrDefaultAsync(o => o.OrderUniqueIdentifier == returnRequest.OrderUniqueIdentifier);

            if(existingOrder == null)
            {
                return new Response_ReturnRequest()
                {
                    isSuccess = false,
                    Message = "No order found ith given Order Unique Identifier"
                };
            }

            ReturnRequestFromUser returnRequestFromUser = new ReturnRequestFromUser()
            {
                OrderUniqueIdentifier = returnRequest.OrderUniqueIdentifier,
                ExchangeUniqueIdentifier = await GenerateUniqueExchangeIdentifier(),
                ExchangeRequestTime = DateTime.UtcNow,
                UserId = userId,
                Email = returnRequest.Email,
                PhoneNumber = returnRequest.PhoneNumber,
                BankName = returnRequest.BankName,
                AccountNumber = returnRequest.AccountNumber,
                Message = returnRequest.Message,
            };

            await _db.returnRequestsFromUsers.AddAsync(returnRequestFromUser);
            await _db.SaveChangesAsync();

            return new Response_ReturnRequest()
            {
                isSuccess = true,
                Message = "Return Request Added Successfully"
            };
        }

        #region Helper Functions

        private async Task<string> GenerateUniqueExchangeIdentifier()
        {
            char[] letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            Random random = new Random();
            string randomLetter = "";
            for(int i  =  0; i < 3; i++)
            {
                randomLetter += letters[random.Next(letters.Length)];
            }

            int randomNumber = random.Next(100000000, 999999999);

            string exchangeUniqueIdentifier = randomLetter + randomNumber.ToString();

            var existingIdentifierInReturns = await _db.returnRequestsFromUsers
                .FirstOrDefaultAsync(rr => rr.ExchangeUniqueIdentifier == exchangeUniqueIdentifier);

            if(existingIdentifierInReturns != null)
            {
                GenerateUniqueExchangeIdentifier();
            }

            return exchangeUniqueIdentifier;
        }


        #endregion




    }
}
