using ECommerceNet8.Data;
using ECommerceNet8.DTOs.RequestExchangeDtos.Request;
using ECommerceNet8.DTOs.RequestExchangeDtos.Response;
using ECommerceNet8.Models.OrderModels;
using ECommerceNet8.Models.ReturnExchangeModels;
using ECommerceNet8.Repositories.ReturnExchangeRequestRepository;
using Microsoft.EntityFrameworkCore;

namespace ECommerceNet8.Repositories.ReturnExchangeRequestRepository
{
    public class ReturnExchangeRequestRepository : IReturnExchangeRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public ReturnExchangeRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        //FOR USERS
        public async Task<ICollection<ExchangeRequestFromUser>> GetExchangeRequestFromUsers()
        {
            var existingExchangeRequest = await _db.exchangeRequestsFromUsers
                .ToListAsync();

            return existingExchangeRequest;
        }
        public async Task<ExchangeRequestFromUser> GetExchangeRequestByExchangeUniqueId(string exchangeUniqueIdentifier)
        {
            var existingExchangeRequest = await _db.exchangeRequestsFromUsers
                .FirstOrDefaultAsync(er=>er.ExchangeUniqueIdentifier  ==  exchangeUniqueIdentifier);

            return existingExchangeRequest;
        }
        public async Task<ICollection<ExchangeRequestFromUser>> GetExchangeRequestByOrderUniqueIdentifier(string orderUniqueIdentifier)
        {
            var existingExchangeRequest = await _db.exchangeRequestsFromUsers
                .Where(er=>er.OrderUniqueIdentifier == orderUniqueIdentifier).ToListAsync();

            return existingExchangeRequest;
        }
        public async Task<Response_ExchangeRequest> AddExchangeRequest(Request_ExchangeRequest exchangeRequest, string UserId)
        {
            var existingOrder = await _db.Orders
                .FirstOrDefaultAsync(o => o.OrderUniqueIdentifier == exchangeRequest.OrderUniqueIdentifier);

            if (existingOrder == null)
            {
                return new Response_ExchangeRequest()
                {
                    isSuccess = false,
                    Message = "No Order Found With Given Unique Identifier"
                };
            }

            var exchangeRequestFromUser = new ExchangeRequestFromUser()
            {
                OrderUniqueIdentifier = exchangeRequest.OrderUniqueIdentifier,
                ExchangeUniqueIdentifier = await GenerateUniqueExchangeIdentifierForExchange(),
                ExchangeRequestTime = DateTime.UtcNow,
                UserId = UserId,
                Email = exchangeRequest.Email,
                PhoneNumber = exchangeRequest.PhoneNumber,
                ApartmentNumber = exchangeRequest.ApartmentNumber,
                HouseNumber = exchangeRequest.HouseNumber,
                Street = exchangeRequest.Street,
                City = exchangeRequest.City,
                Region = exchangeRequest.Region,
                Country = exchangeRequest.Country,
                PostalCode = exchangeRequest.PostalCode,
                Message = exchangeRequest.Message
            };

            await _db.exchangeRequestsFromUsers.AddAsync(exchangeRequestFromUser);
            await _db.SaveChangesAsync();

            return new Response_ExchangeRequest()
            {
                isSuccess = true,
                Message = "Exchange Request Added Successfully"
            };
        }


        public async Task<Response_IsSuccess> AddItemToReturn(int itemAtCustomerId, int quantity)
        {
            //TODO Check if item at customer exist(DONE)
            var itemAtCustomer = await _db.ItemAtCustomers
                .FirstOrDefaultAsync(ic=>ic.Id == itemAtCustomerId);
            if(itemAtCustomer == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No Item At Customer Found With Given Id"
                };
            }
            //TODO Check if enough items at customer to add to returns
            if(itemAtCustomer.Quantity < quantity)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = $"Not Enough Items At Customer: {itemAtCustomer.Quantity}" +
                    $"Requested Quantity:{quantity}"
                };
            }
            //TODO Add item to returnedItems if productVariant exist add quantity

            var existingReturnItem = await _db.returnedItemsFromCustomers
                .FirstOrDefaultAsync(ri=>ri.OrderId == itemAtCustomer.OrderId
                && ri.ProductVariantId == itemAtCustomer.ProductVariantId);

            if(existingReturnItem == null)
            {
                var returnedItemFromCustomer = new ReturnedItemsFromCustomer()
                {
                    OrderId = itemAtCustomer.OrderId,
                    BaseProductId = itemAtCustomer.BaseProductId,
                    BaseProductName = itemAtCustomer.BaseProductName,
                    ProductVariantId = itemAtCustomer.ProductVariantId,
                    ProductVariantColor = itemAtCustomer.ProductVariantColor,
                    ProductVariantSize = itemAtCustomer.ProductVariantSize,
                    PricePerItem = itemAtCustomer.PricePaidPerItem,
                    Quantity = quantity
                };

                await _db.returnedItemsFromCustomers.AddAsync(returnedItemFromCustomer);
                await _db.SaveChangesAsync();
            }
            else
            {
                existingReturnItem.Quantity = existingReturnItem.Quantity + quantity;
                await _db.SaveChangesAsync();
            }
            //TODO Remove quantity from itemAtCustomer if == 0 remove item
            itemAtCustomer.Quantity = itemAtCustomer.Quantity - quantity;
            if(itemAtCustomer.Quantity  == 0)
            {
                _db.ItemAtCustomers.Remove(itemAtCustomer);
            }
            await _db.SaveChangesAsync();

            return new Response_IsSuccess()
            {
                isSuccess = true,
                Message = "Item Successfully Moved To Returned Items"
            };
        }

        public async Task<Response_IsSuccess> RemoveItemFromReturn(int returnedItemId, int quantity)
        {
            //TODO check if item exist
            var existingReturnedItem = await _db.returnedItemsFromCustomers
                .FirstOrDefaultAsync(ri => ri.Id == returnedItemId);
            if(existingReturnedItem == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No Item In Returned Items Found With Given Id"
                };
            }
            // TODO Check if enough item to remove from returned items
            if(existingReturnedItem.Quantity < quantity)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Not Enough Quantity To Remove"
                };
            }
            // TODO Add Items In ItemsAtCustomer if items exist add quantity
            var existingItemAtCustomer = await _db.ItemAtCustomers
                .FirstOrDefaultAsync(ic => ic.OrderId == existingReturnedItem.OrderId
                && ic.ProductVariantId == existingReturnedItem.ProductVariantId);

            if(existingItemAtCustomer == null)
            {
                var itemAtCustomer = new ItemAtCustomer()
                {
                    OrderId = existingReturnedItem.OrderId,
                    BaseProductId = existingReturnedItem.BaseProductId,
                    BaseProductName = existingReturnedItem.BaseProductName,
                    ProductVariantId = existingReturnedItem.ProductVariantId,
                    ProductVariantColor = existingReturnedItem.ProductVariantColor,
                    ProductVariantSize = existingReturnedItem.ProductVariantSize,
                    PricePaidPerItem = existingReturnedItem.PricePerItem,
                    Quantity = quantity
                };

                await _db.ItemAtCustomers.AddAsync(itemAtCustomer);
                await _db.SaveChangesAsync();
            }
            else
            {
                existingItemAtCustomer.Quantity = existingItemAtCustomer.Quantity + quantity;
                await _db.SaveChangesAsync();
            }
            // TODO Remove Quantity from returnedItem if qty <= 0, remove item
            existingReturnedItem.Quantity = existingReturnedItem.Quantity - quantity;
            if(existingReturnedItem.Quantity == 0)
            {
                _db.returnedItemsFromCustomers.Remove(existingReturnedItem);
            }
            await _db.SaveChangesAsync();

            return new Response_IsSuccess()
            {
                isSuccess = true,
                Message = "Item Moved To Items At Customer"
            };


        }
        public async Task<Response_ReturnedItems> GetAllReturnedItems(int orderId)
        {
            var existingOrder = await _db.Orders
                .Include(o => o.ReturnedItemsFromCustomers)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (existingOrder == null)
            {
                return new Response_ReturnedItems()
                {
                    IsSuccess = false,
                    Message = "No Order Exist With Given OrderId"
                };
            }

            return new Response_ReturnedItems()
            {
                IsSuccess = true,
                Message = "All Returned Items From Customer",
                returnedItemsFromCustomers = existingOrder.ReturnedItemsFromCustomers.ToList()

            };
        }

      





        public Task<Response_Exchange> CreateExchangeRequest(Request_Exchange exchangeRequest)
        {
            throw new NotImplementedException();
        }

        public Task<Response_Exchange> CreateExchangeRequestByAdmin(Request_ExchangeByAdmin exchangeRequest)
        {
            throw new NotImplementedException();
        }

        public Task<Response_ExchangeFullInfo> GetExchangeRequest(string exchangeUniqueIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<Response_IsSuccess> MarkExchangeOrderAsDone(string exchangeUniqueIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<Response_IsSuccess> MarkExchangeOrderAsNotDone(string exchangeUniqueIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<Response_Exchange> SendEmailWithPendingInfo(string exchangeUniqueIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<Response_Exchange> SendEmailWithCompletedPdf(string exchangeUniqueIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<Response_AllExchangedGoodItems> GetAllExchangeGoodItems(string exchangeUniqueIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<Response_IsSuccess> AddExchangeGoodItem(Request_AddExchangeGoodItem exchangeGoodItem)
        {
            throw new NotImplementedException();
        }

        public Task<Response_IsSuccess> RemoveExchangeGoodItem(Request_RemoveExchangeGoodItem exchangeGoodItem)
        {
            throw new NotImplementedException();
        }

        public Task<Response_AllExchangePendingItems> GetAllExchangePendingItems(string exchangeUniqueIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<Response_IsSuccess> AddExchangePendingItem(Request_AddExchangePendingItem exchangePendingItem)
        {
            throw new NotImplementedException();
        }

        public Task<Response_IsSuccess> RemoveExchangePendingItem(Request_RemoveExchangePendingItem exchangePendingItem)
        {
            throw new NotImplementedException();
        }

        public Task<Response_IsSuccess> MovePendingItemToGood(Request_MovePendingToGood movePendingToGood)
        {
            throw new NotImplementedException();
        }

        public Task<Response_IsSuccess> MovePendingItemToBad(Request_MovePendingToBad movePendingToBad)
        {
            throw new NotImplementedException();
        }

        public Task<Response_AllExchangeBadItems> GetAllExchangeBadItems(string exchangeUniqueIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<Response_IsSuccess> AddExchangeBadItem(Request_AddExchangeBadItem addExchangeBadItem)
        {
            throw new NotImplementedException();
        }

        public Task<Response_IsSuccess> RemoveExchangeBadItem(Request_RemoveExchangeBadItem removeExchangeBadItem)
        {
            throw new NotImplementedException();
        }


        #region HelperFunctions
        private async Task<string> GenerateUniqueExchangeIdentifierForExchange()
        {
            char[] letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            Random random = new Random();
            string ramdomLetters = "";

            for(int i = 0; i < 3; i++)
            {
                ramdomLetters += letters[random.Next(letters.Length)];
            }

            int randomNumber = random.Next(100000000, 999999999);

            string ExchangeUniqueIdentifier = ramdomLetters + randomNumber.ToString();

            var existingIdentifierInReturns = await _db.exchangeRequestsFromUsers
                .FirstOrDefaultAsync(er => er.ExchangeUniqueIdentifier == ExchangeUniqueIdentifier);
            if(existingIdentifierInReturns != null)
            {
                GenerateUniqueExchangeIdentifierForExchange();
            }

            return ExchangeUniqueIdentifier;
        }
        #endregion

    }
}
