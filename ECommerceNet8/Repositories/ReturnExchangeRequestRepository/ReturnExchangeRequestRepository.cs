using ECommerceNet8.Data;
using ECommerceNet8.DTOConvertions;
using ECommerceNet8.DTOs.RequestExchangeDtos.Request;
using ECommerceNet8.DTOs.RequestExchangeDtos.Response;
using ECommerceNet8.Models.OrderModels;
using ECommerceNet8.Models.ReturnExchangeModels;
using ECommerceNet8.Repositories.ReturnExchangeRequestRepository;
using ECommerceNet8.Templates;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Globalization;

namespace ECommerceNet8.Repositories.ReturnExchangeRequestRepository
{
    public class ReturnExchangeRequestRepository : IReturnExchangeRequestRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly ISendGridClient _sendGridClient;

        public ReturnExchangeRequestRepository(ApplicationDbContext db,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration,
            ISendGridClient sendGridClient)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _sendGridClient = sendGridClient;
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

        public async Task<Response_Exchange> CreateExchangeRequest(Request_Exchange exchangeRequest)
        {
            int apartmentNumber;
            if(exchangeRequest.ApartmentNumber == null
                || exchangeRequest.ApartmentNumber == 0)
            {
                apartmentNumber = 0;
            }
            else
            {
                apartmentNumber = (int)exchangeRequest.ApartmentNumber;
            }

            var orderInDb = await _db.Orders
                .FirstOrDefaultAsync(o => o.OrderUniqueIdentifier == exchangeRequest.OrderUniqueIdentfier);

            if(orderInDb == null)
            {
                return new Response_Exchange()
                {
                    isSuccess = false,
                    Message = "No order found with given Unique Identifier"
                };
            }
            var itemExchangeRequest = new ItemExchangeRequest()
            {
                OrderId = orderInDb.OrderId,
                OrderUniqueIdentifier = exchangeRequest.OrderUniqueIdentfier,
                ExchangeUniqueIdentifier = exchangeRequest.ExchangeUniqueIdentfier,
                AdminId = exchangeRequest.AdminId,
                AdminName = exchangeRequest.AdminFullName,
                UserEmail = exchangeRequest.UserEmail,
                UserPhone = exchangeRequest.UserPhone,
                UserFirstName = exchangeRequest.UserFirstName,
                UserLastName = exchangeRequest.UserLastName,
                ApartmentNumber = apartmentNumber,
                HouseNumber = exchangeRequest.HouseNumber,
                Street = exchangeRequest.Street,
                City = exchangeRequest.City,
                Region = exchangeRequest.Region,
                Country = exchangeRequest.Country,
                PostalCode = exchangeRequest.PostalCode,
                RequestClosed = false
            };

            await _db.ItemExchangeRequests.AddAsync(itemExchangeRequest);
            await _db.SaveChangesAsync();

            return new Response_Exchange()
            {
                isSuccess = true,
                ExchangeRequestId = itemExchangeRequest.Id,
                OrderUniqueIdentifier = exchangeRequest.OrderUniqueIdentfier,
                ExchangeUniqueIdentfier = exchangeRequest.ExchangeUniqueIdentfier,
                Message = "Exchange Request Created Successfully"
            };

        }

        public async Task<Response_ExchangeFullInfo> GetExchangeRequest(string exchangeUniqueIdentifier)
        {
            var exchangeRequest = await _db.ItemExchangeRequests
                .Include(ie=>ie.exchangeOrderItems)
                .Include(ie=>ie.exchangeItemsCanceled)
                .Include(ie=>ie.exchangeItemsPending)
                .Include(ie=>ie.exchangeConfirmedPdfInfo)
                .FirstOrDefaultAsync(ie=>ie.ExchangeUniqueIdentifier == exchangeUniqueIdentifier);

            if(exchangeRequest  == null)
            {
                return new Response_ExchangeFullInfo()
                {
                    IsSuccess = false,
                    Message = "No Exchange Request Found With Given Unique Id"
                };
            }

            return new Response_ExchangeFullInfo()
            {
                IsSuccess = true,
                Message = "Exchange Retrieved",
                OrderId = exchangeRequest.OrderId,
                OrderUniqueIdentifier = exchangeRequest.OrderUniqueIdentifier,
                ExchangeUniqueIdentfier = exchangeRequest.ExchangeUniqueIdentifier,
                AdminId = exchangeRequest.AdminId,
                AdminFullName = exchangeRequest.AdminName,
                UserFirstName = exchangeRequest.UserFirstName,
                UserLastName = exchangeRequest.UserLastName,
                UserEmail = exchangeRequest.UserEmail,
                UserPhone = exchangeRequest.UserPhone,
                ApartmentNumber = exchangeRequest.ApartmentNumber,
                HouseNumber = exchangeRequest.HouseNumber,
                Street = exchangeRequest.Street,
                City = exchangeRequest.City,
                Region = exchangeRequest.Region,
                Country = exchangeRequest.Country,
                PostalCode = exchangeRequest.PostalCode,
                ExchangeOrderItems = exchangeRequest.exchangeOrderItems.ToList(),
                ExchangeItemsCanceled = exchangeRequest.exchangeItemsCanceled.ToList(),
                exchangeItemsPending = exchangeRequest.exchangeItemsPending.ToList(),
                RequestClosed = exchangeRequest.RequestClosed,
                exchangeConfirmedPdfInfoId = exchangeRequest.ExchangeConfirmedPdfInfoId,
                ExchangeConfirmedPdfInfo = exchangeRequest.exchangeConfirmedPdfInfo
            };
        }

        public async Task<Response_IsSuccess> MarkExchangeOrderAsDone(string exchangeUniqueIdentifier)
        {
            //TODO check if exchangeOrder Exist
            var existingExchangeOrder = await _db.ItemExchangeRequests
                .Include(ie => ie.exchangeOrderItems)
                .Include(ie => ie.exchangeItemsPending)
                .Include(ie => ie.exchangeItemsCanceled)
                .FirstOrDefaultAsync(ie => ie.ExchangeUniqueIdentifier == exchangeUniqueIdentifier);

            if(existingExchangeOrder == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No existing Exchange order found with given Unique Id"
                };
            }
            //TODO check if where is pending items
            if(existingExchangeOrder.exchangeItemsPending.Count > 0)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Order Has Pending Items"
                };
            }
            //TODO check if exchange order is closed
            if(existingExchangeOrder.RequestClosed == true)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = " Exchange Order Already Closed"
                };
            }
            //TODO check if pdf already exist
            var order = await _db.Orders
                .FirstOrDefaultAsync(o => o.OrderId == existingExchangeOrder.OrderId);
            var pdfExist = await _db.ExchangeConfirmedPdfInfos
                .FirstOrDefaultAsync(pdf => pdf.ItemExchangeRequestId == existingExchangeOrder.Id);

            if(pdfExist != null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "PDF already exist, if you want to change id, delete existing one first"
                };
            }
            //TODO check if itemAtCustomer exist with given productVariantId, add quantity or item
            foreach(var exchangeItem in existingExchangeOrder.exchangeOrderItems)
            {
                var itemAtCustomer = await _db.ItemAtCustomers
                    .FirstOrDefaultAsync(ic => ic.OrderId == existingExchangeOrder.OrderId
                    && ic.ProductVariantId == exchangeItem.ExchangedProductVariantId);
                if(itemAtCustomer == null)
                {
                    ItemAtCustomer newItemAtCustomer = new ItemAtCustomer()
                    {
                        OrderId = existingExchangeOrder.OrderId,
                        BaseProductId = exchangeItem.BaseProductId,
                        BaseProductName = exchangeItem.BaseProductName,
                        ProductVariantId = exchangeItem.ExchangedProductVariantId,
                        ProductVariantColor = exchangeItem.ExchangedProductVariantColor,
                        ProductVariantSize = exchangeItem.ExchangedProductVariantSize,
                        PricePaidPerItem = exchangeItem.PricePerItemPaid,
                        Quantity = exchangeItem.Quantity,
                    };

                    await _db.ItemAtCustomers.AddAsync(newItemAtCustomer);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    itemAtCustomer.Quantity = itemAtCustomer.Quantity + exchangeItem.Quantity;
                    await _db.SaveChangesAsync();
                }
            }
            //TODO generate pdf
            ExchangeConfirmedPdfInfo pdfInfo  = new ExchangeConfirmedPdfInfo();
            pdfInfo.ItemExchangeRequestId = existingExchangeOrder.Id;
            pdfInfo.Name = "PDF info for " + existingExchangeOrder.ExchangeUniqueIdentifier
                + " order";
            pdfInfo.Added = DateTime.UtcNow;
            pdfInfo.Path = await CreateExchangePdf(existingExchangeOrder, order.UserId);

            await _db.ExchangeConfirmedPdfInfos.AddAsync(pdfInfo);
            await _db.SaveChangesAsync();

            existingExchangeOrder.ExchangeConfirmedPdfInfoId = pdfInfo.Id;
            existingExchangeOrder.RequestClosed = true;
            await _db.SaveChangesAsync();

            return new Response_IsSuccess()
            {
                isSuccess = true,
                Message = "Exchange order confirmed, pdf created, items at customer added"
            };
             

        }
        public async Task<Response_IsSuccess> MarkExchangeOrderAsNotDone(string exchangeUniqueIdentifier)
        {
            //CHECK IF EXCHANGE ORDER EXIST
            var existingExchangeOrder = await _db.ItemExchangeRequests
                .Include(ie => ie.exchangeOrderItems)
                .FirstOrDefaultAsync(ie => ie.ExchangeUniqueIdentifier == exchangeUniqueIdentifier);

            if( existingExchangeOrder == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No exisitng exchange order found with givne Unique Id"
                };
            }
            //CHECK IF ORDER IS CLOSED
            if(existingExchangeOrder.RequestClosed == false)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Exchange order is not closed"
                };
            }

            //CHECK IF ENOUGH ITEMS AT CUSTOMER TO CANCEL EXCHANGE ORDER
            bool allItemsExistInCustomerItems = true;
            bool allHaveEnoughQty = true;

            foreach(var exchangeItem in existingExchangeOrder.exchangeOrderItems)
            {
                var itemAtCustomer = await _db.ItemAtCustomers
                    .FirstOrDefaultAsync(ic => ic.OrderId == existingExchangeOrder.OrderId
                    && ic.ProductVariantId == exchangeItem.ExchangedProductVariantId);
                if(itemAtCustomer == null )
                {
                    allItemsExistInCustomerItems = false;
                }
                else
                {
                    if(itemAtCustomer.Quantity <  exchangeItem.Quantity)
                    {
                        allHaveEnoughQty = false;
                    }
                }
            }

            if(allItemsExistInCustomerItems == false)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = " Some items is not at itemsAtCustomer"
                };
            }
            if(allHaveEnoughQty == false)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Not all items have enough quantity to be returned"
                };
            }

            //REMOVE ITEMS AT CUSTOMER, IF QTY == 0, REMOVE ITEM
            foreach(var exchangeItem in existingExchangeOrder.exchangeOrderItems)
            {
                var itemAtCustomer = await _db.ItemAtCustomers
                    .FirstOrDefaultAsync(ic => ic.OrderId == existingExchangeOrder.OrderId
                    && ic.ProductVariantId == exchangeItem.ExchangedProductVariantId);

                itemAtCustomer.Quantity = itemAtCustomer.Quantity - exchangeItem.Quantity;
                if(itemAtCustomer.Quantity  == 0)
                {
                    _db.ItemAtCustomers.Remove(itemAtCustomer);
                }
                await _db.SaveChangesAsync();
            }

            //DELETE PDF
            var existingPdfInfo = await _db.ExchangeConfirmedPdfInfos
            .FirstOrDefaultAsync(pdf => pdf.ItemExchangeRequestId == existingExchangeOrder.Id);

            if(existingPdfInfo != null)
            {
                if(File.Exists(existingPdfInfo.Path))
                {
                    File.Delete(existingPdfInfo.Path);
                }

                _db.ExchangeConfirmedPdfInfos.Remove(existingPdfInfo);
                await _db.SaveChangesAsync();
            }

            existingExchangeOrder.RequestClosed = false;
            await _db.SaveChangesAsync();

            return new Response_IsSuccess()
            {
                isSuccess = false,
                Message = " Exchange request canceled"
            };
        }

        public async Task<Response_Exchange> SendEmailWithPendingInfo(string exchangeUniqueIdentifier)
        {
            var existingExchangeOrder = await _db.ItemExchangeRequests
                .Include(ie => ie.exchangeOrderItems)
                .Include(ie => ie.exchangeItemsPending)
                .Include(ie => ie.exchangeItemsCanceled)
                .FirstOrDefaultAsync(ie => ie.ExchangeUniqueIdentifier == exchangeUniqueIdentifier);

            if(existingExchangeOrder == null)
            {
                return new Response_Exchange()
                {
                    isSuccess = false,
                    Message = "No Existing exchange order found with given Unique Id"
                };
            }

            string fromEmail = _configuration.GetSection("SendGridEmailSettings")
                .GetValue<string>("FromEmail");
            string fromName = _configuration.GetSection("SendGridEmailSettings")
                .GetValue<string>("FromName");

            var msg = new SendGridMessage()
            {
                From = new EmailAddress(fromEmail, fromName),
                Subject = "Items Refund Details",
                HtmlContent = EmailTemplates.ExchangePendingTemplate(existingExchangeOrder)
            };

            var email = existingExchangeOrder.UserEmail; //DONT USE FOR TESTING
            msg.AddTo("vaceintech@gmail.com");

            var response = await _sendGridClient.SendEmailAsync(msg);

            string message = response.IsSuccessStatusCode ? "Email sent successfully" :
                "Email Failed To Send";

            bool messageSuccess = response.IsSuccessStatusCode;

            return new Response_Exchange()
            {
                isSuccess = messageSuccess,
                Message = message
            };
           
        }
        public async Task<Response_Exchange> SendEmailWithCompletedPdf(string exchangeUniqueIdentifier)
        {
            //TODO check if exchange exist
            var existingExchangeOrder = await _db.ItemExchangeRequests
                .Include(ie => ie.exchangeConfirmedPdfInfo)
                .Include(ie => ie.exchangeOrderItems)
                .Include(ie => ie.exchangeItemsCanceled)
                .Include(ie => ie.exchangeItemsPending)
                .FirstOrDefaultAsync(ie => ie.ExchangeUniqueIdentifier == exchangeUniqueIdentifier);

            if(existingExchangeOrder ==  null)
            {
                return new Response_Exchange()
                {
                    isSuccess = false,
                    Message = "No existing exchange order found with given Unique Id"
                };
            }
            //TODO check if order is closed
            if(existingExchangeOrder.RequestClosed == false)
            {
                return new Response_Exchange()
                {
                    isSuccess = false,
                    Message = "Exchange order is not closed, close exchange first"
                };
            }
            //TODO send email with attached pdf
            string fromEmail = _configuration.GetSection("SendGridEmailSettings")
                .GetValue<string>("FromEmail");
            string fromName = _configuration.GetSection("SendGridEmailSettings")
                .GetValue<string>("FromName");

            var msg = new SendGridMessage()
            {
                From = new EmailAddress(fromEmail, fromName),
                Subject = "Item exchange details",
                HtmlContent = EmailTemplates.ExchangePendingTemplate(existingExchangeOrder)
            };

            var email = existingExchangeOrder.UserEmail;//DONT USE FOR TESTING

            msg.AddTo("vaceintech@gmail.com");
            var bytes = File.ReadAllBytes(existingExchangeOrder.exchangeConfirmedPdfInfo.Path);
            var file  = Convert.ToBase64String(bytes);
            msg.AddAttachment("Invoice.pdf", file);

            var response = await _sendGridClient.SendEmailAsync(msg);
            string message = response.IsSuccessStatusCode ? "Email sent successfully" :
                "Email failed to send";

            bool messageSuccess = response.IsSuccessStatusCode;

            return new Response_Exchange()
            {
                isSuccess = messageSuccess,
                Message = message
            };
        }

        public Task<Response_Exchange> CreateExchangeRequestByAdmin(Request_ExchangeByAdmin exchangeRequest)
        {
            throw new NotImplementedException();
        }

       

        public async Task<Response_AllExchangedGoodItems> GetAllExchangeGoodItems(string exchangeUniqueIdentifier)
        {
            var existingExchangeOrder = await _db.ItemExchangeRequests
                .Include(ie => ie.exchangeOrderItems)
                .FirstOrDefaultAsync(ie => ie.ExchangeUniqueIdentifier == exchangeUniqueIdentifier);

            if (existingExchangeOrder == null)
            {
                return new Response_AllExchangedGoodItems()
                {
                    IsSuccess = false,
                    Message = "No exchange order found with given Uniqe Id"
                };
            }

            var allExchangeGoodItemList = existingExchangeOrder.ConvertToDtoGoodItems();

            allExchangeGoodItemList.IsSuccess = true;
            allExchangeGoodItemList.Message = "Items Good For Exchange";

            return allExchangeGoodItemList;

        }

        public async Task<Response_IsSuccess> AddExchangeGoodItem(Request_AddExchangeGoodItem exchangeGoodItem)
        {
            //TODO check if exchange order exist
            var existingExchangeOrder = await _db.ItemExchangeRequests
                .FirstOrDefaultAsync(ie=>ie.ExchangeUniqueIdentifier== exchangeGoodItem.ExchangeUniqueIdentifier);
            if(existingExchangeOrder == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No Exchange order found with given Unique Id"
                };
            }
            //TODO check if exchange order not closed
            if(existingExchangeOrder.RequestClosed == true)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Order is already closed, cant modify items"
                };
            }
            //TODO check if order exist
            var existingOrder = await _db.Orders
                .Include(o=>o.ReturnedItemsFromCustomers)
                .FirstOrDefaultAsync(o=>o.OrderUniqueIdentifier 
                == exchangeGoodItem.OrderUniqueIdentifier);
            if(existingOrder == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No Order Found With Given Unique Id"
                };
            }
            //TODO check if returnItem exist and has enough quantity
            bool itemAtCustomerExist = false;
            bool itemAtCustomerHasEnoughQty = false;
            foreach(var item in existingOrder.ReturnedItemsFromCustomers)
            {
                if(item.ProductVariantId == exchangeGoodItem.ReturnedProductVariantId)
                {
                    itemAtCustomerExist = true;
                    if(item.Quantity >= exchangeGoodItem.Quantity)
                    {
                        itemAtCustomerHasEnoughQty = true;
                    }
                }
            }

            if(itemAtCustomerExist == false)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Item Returned From Customer does not exist with given " +
                    "Return Product Variant Id"
                };
            }
            if(itemAtCustomerHasEnoughQty == false)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Not Enough Items To Exchange At Customer"
                };
            }
            //TODO check if product variant have enough qty
            var exchangeProductVariant = await _db.ProductVariants
                .Include(pv => pv.baseProduct)
                .Include(pv => pv.productColor)
                .Include(pv => pv.productSize)
                .FirstOrDefaultAsync(pv => pv.Id == exchangeGoodItem.ExchangeProductVariantId);
            if(exchangeProductVariant.Quantity < exchangeGoodItem.Quantity)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Not enough items in storage to exchange"
                };
            }
            // TODO Remove quantity/ item from returnedItemFromCustomer
            var itemReturned = await _db.returnedItemsFromCustomers
                .FirstOrDefaultAsync(r => r.OrderId == existingExchangeOrder.OrderId
                && r.ProductVariantId == exchangeGoodItem.ReturnedProductVariantId);


            //TODO Check if items exist
            if(itemReturned  == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No item at returned items with given product variant id"
                };
            }
            var pricePerItem = itemReturned.PricePerItem;
            //TODO Remove quantity, if quantity == 0 remove item

            itemReturned.Quantity = itemReturned.Quantity - exchangeGoodItem.Quantity;
            if(itemReturned.Quantity < 0)
            {
                itemReturned.Quantity = itemReturned.Quantity + exchangeGoodItem.Quantity;

                await _db.SaveChangesAsync();

                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Not enough items at returned items, please check quantity"
                };
            }
            if(itemReturned.Quantity == 0)
            {
                _db.returnedItemsFromCustomers.Remove(itemReturned);
            }
            await _db.SaveChangesAsync();
            //TODO Remove Quantity from productVariant (ExchangeProductVariantId)

            await RemoveItemQuantity(exchangeGoodItem.ExchangeProductVariantId,
                exchangeGoodItem.Quantity);

            //TODO Exchange Order check if exist, if does add quantity, if not create new Item

            var returnedProductVariantInfo = await _db.ProductVariants
                .Include(pv => pv.productSize)
                .Include(pv => pv.productColor)
                .FirstOrDefaultAsync(pv => pv.Id == exchangeGoodItem.ReturnedProductVariantId);

            var existingExchangeItem = await _db.ExchangeOrderItems
                .FirstOrDefaultAsync(eo => eo.ItemExchangeRequestId == existingExchangeOrder.Id
                && eo.ReturnedProductVariantId == returnedProductVariantInfo.Id
                && eo.ExchangedProductVariantId == exchangeProductVariant.Id);
            if(existingExchangeItem != null)
            {
                existingExchangeItem.Quantity = existingExchangeItem.Quantity +
                    exchangeGoodItem.Quantity;

                await _db.SaveChangesAsync();
            }
            else
            {
                var exchangeOrderItem = new ExchangeOrderItem();
                exchangeOrderItem.ItemExchangeRequestId = existingExchangeOrder.Id;
                exchangeOrderItem.BaseProductId = exchangeProductVariant.BaseProductId;
                exchangeOrderItem.BaseProductName = exchangeProductVariant.baseProduct.Name;
                exchangeOrderItem.ReturnedProductVariantId = returnedProductVariantInfo.Id;
                exchangeOrderItem.ReturnedProductVariantColor = 
                    returnedProductVariantInfo.productColor.Name;
                exchangeOrderItem.ReturnedProductVariantSize =
                    returnedProductVariantInfo.productSize.Name;
                exchangeOrderItem.ExchangedProductVariantId = exchangeProductVariant.Id;
                exchangeOrderItem.ExchangedProductVariantColor =
                    exchangeProductVariant.productColor.Name;
                exchangeOrderItem.ExchangedProductVariantSize =
                    exchangeProductVariant.productSize.Name;
                exchangeOrderItem.PricePerItemPaid = pricePerItem;
                exchangeOrderItem.Quantity = exchangeGoodItem.Quantity;
                exchangeOrderItem.Message = "Item good for exchange";

                await _db.ExchangeOrderItems.AddAsync(exchangeOrderItem);
                await _db.SaveChangesAsync();
                  
            }

            return new Response_IsSuccess()
            {
                isSuccess = true,
                Message = "Item Exchange Added Successfully"
            };

        }

        public async Task<Response_IsSuccess> RemoveExchangeGoodItem(Request_RemoveExchangeGoodItem exchangeGoodItem)
        {
            //TODO get exchange item
            var exchangeGoodItemFromDb = await _db.ExchangeOrderItems
                .FirstOrDefaultAsync(eo => eo.Id == exchangeGoodItem.ExchangeItemId);

            if(exchangeGoodItemFromDb == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No Item Found With Given Exchange Item Id"
                };
            }
            //TODO check if exchange request is closed
            var exchangeOrder = await _db.ItemExchangeRequests
                .FirstOrDefaultAsync(ie => ie.Id == exchangeGoodItemFromDb.ItemExchangeRequestId);
            if(exchangeOrder.RequestClosed == true)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Exchange order is closed and cant be modified"
                };
            }
            //TODO Check if enough quantity
            
            if(exchangeGoodItemFromDb.Quantity < exchangeGoodItem.Quantity)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Exchange order does not have enough items to return"
                };
            }


            var pricePerItem = exchangeGoodItemFromDb.PricePerItemPaid;
            //TODO add quantity to ReturnedItemsFromCustomer with returnProductVariantId
            var returnItemAtCustomer = await _db.returnedItemsFromCustomers
                .FirstOrDefaultAsync(ri => ri.OrderId == exchangeOrder.OrderId
                && ri.ProductVariantId == exchangeGoodItemFromDb.ReturnedProductVariantId);

            if(returnItemAtCustomer == null)
            {
                var returnedItemFromCustomer = new ReturnedItemsFromCustomer();

                returnedItemFromCustomer.OrderId= exchangeOrder.OrderId;

                returnedItemFromCustomer.BaseProductId  = 
                    exchangeGoodItemFromDb.BaseProductId;

                returnedItemFromCustomer.BaseProductName = 
                    exchangeGoodItemFromDb.BaseProductName;

                returnedItemFromCustomer.ProductVariantId =
                    exchangeGoodItemFromDb.ReturnedProductVariantId;

                returnedItemFromCustomer.ProductVariantColor =
                    exchangeGoodItemFromDb.ReturnedProductVariantColor;

                returnedItemFromCustomer.ProductVariantSize =
                    exchangeGoodItemFromDb.ReturnedProductVariantSize;

                returnedItemFromCustomer.PricePerItem = pricePerItem;

                returnedItemFromCustomer.Quantity = exchangeGoodItem.Quantity;

                await _db.returnedItemsFromCustomers.AddAsync(returnedItemFromCustomer);
                await _db.SaveChangesAsync();
            }
            else
            {
                returnItemAtCustomer.Quantity = returnItemAtCustomer.Quantity 
                    + exchangeGoodItem.Quantity;
            }

            //TODO return back quantity to productVariant with ExchangeProductVariant
            await AddItemQuantity(exchangeGoodItemFromDb.ExchangedProductVariantId,
                exchangeGoodItemFromDb.Quantity);
            //TODO remove  ExchangeItem
            _db.ExchangeOrderItems.Remove(exchangeGoodItemFromDb);
            await _db.SaveChangesAsync();

            return new Response_IsSuccess()
            {
                isSuccess = true,
                Message = "Exchange items canceled successfully"
            };

        }

        public async Task<Response_AllExchangePendingItems> GetAllExchangePendingItems(string exchangeUniqueIdentifier)
        {
            var existingExchange = await _db.ItemExchangeRequests
                .Include(ie => ie.exchangeItemsPending)
                .FirstOrDefaultAsync(ie => ie.ExchangeUniqueIdentifier == exchangeUniqueIdentifier);

            if(existingExchange == null)
            {
                return new Response_AllExchangePendingItems()
                {
                    isSuccess = false,
                    Message = "No exchange request found with given Exchange Unique Id"
                };
            }

            var exchangePendingItemsResponse = existingExchange.ConverToDtoPendingItems();
            exchangePendingItemsResponse.isSuccess = true;
            exchangePendingItemsResponse.Message = "Items Retrieved Successfully";

            return exchangePendingItemsResponse;
        }

        public async Task<Response_IsSuccess> AddExchangePendingItem(Request_AddExchangePendingItem exchangePendingItem)
        {
            //TODO check existing exchange Order
            var existingExchange = await  _db.ItemExchangeRequests
                .FirstOrDefaultAsync(ie=>ie.ExchangeUniqueIdentifier ==
                exchangePendingItem.ExchangeUniqueIdentifier);

            if(existingExchange == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No exchange request found with given Unique Id"
                };
            }
            //TODO check existing order 
            var existingOrder = await _db.Orders
                .Include(o => o.ReturnedItemsFromCustomers)
                .FirstOrDefaultAsync(o => o.OrderId == existingExchange.OrderId);

            if(existingOrder == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No order found"
                };
            }
            //TODO check if item exist in Returned Items from customer
            //TODO check if enough quantity for exchange
            bool itemExist = false;
            bool enoughQty = false;

            foreach(var item in existingOrder.ReturnedItemsFromCustomers)
            {
                if(item.ProductVariantId == exchangePendingItem.ReturnedProductVariantId)
                {
                    itemExist = true;
                    if(item.Quantity >=  exchangePendingItem.Quantity)
                    {
                        enoughQty = true;
                    }
                }
            }

            if(itemExist == false)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No item exist at returned items from customer"
                };
            }
            if(enoughQty == false)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Not enough items at returned items to exchange"
                };
            }
            //TODO get returned Item
            var itemAtReturns = await _db.returnedItemsFromCustomers
                .FirstOrDefaultAsync(ri => ri.OrderId == existingOrder.OrderId
                && ri.ProductVariantId == exchangePendingItem.ReturnedProductVariantId);
            //TODO add item to pending
            var existingPendingItem = await _db.ExchangeItemsPending
                .FirstOrDefaultAsync(ei => ei.ItemExchangeRequestId == existingExchange.Id
                && ei.ReturnedProductVariantId == itemAtReturns.ProductVariantId);

            if(existingPendingItem == null)
            {
                ExchangeItemPending exchangeItemPending = new ExchangeItemPending()
                {
                    ItemExchangeRequestId = existingExchange.Id,
                    BaseProductId = itemAtReturns.BaseProductId,
                    BaseProductName = itemAtReturns.BaseProductName,
                    PricePerItemPaid = itemAtReturns.PricePerItem,
                    ReturnedProductVariantId = itemAtReturns.ProductVariantId,
                    ReturnedProductVariantColor = itemAtReturns.ProductVariantColor,
                    ReturnedProductVariantSize = itemAtReturns.ProductVariantSize,
                    Quantity = exchangePendingItem.Quantity,
                    Message = exchangePendingItem.Message
                };

                await _db.ExchangeItemsPending.AddAsync(exchangeItemPending);
                await _db.SaveChangesAsync();
            }
            else
            {
                exchangePendingItem.Quantity = exchangePendingItem.Quantity +
                    exchangePendingItem.Quantity;
                exchangePendingItem.Message = exchangePendingItem.Message;
                await _db.SaveChangesAsync();
            }
            //TODO remove item/quantity from returned items
            itemAtReturns.Quantity = itemAtReturns.Quantity - exchangePendingItem.Quantity;
            if(itemAtReturns.Quantity == 0)
            {
                _db.returnedItemsFromCustomers.Remove(itemAtReturns);
                
            }
            await _db.SaveChangesAsync();
            return new Response_IsSuccess()
            {
                isSuccess = true,
                Message = "Item added to pending Items"
            };

        }

        public async Task<Response_IsSuccess> RemoveExchangePendingItem(Request_RemoveExchangePendingItem exchangePendingItem)
        {
            // TODO check if item is at pending items
            var pendingItem = await _db.ExchangeItemsPending
                .FirstOrDefaultAsync(ei => ei.Id == exchangePendingItem.ExchangeItemId);
            if(pendingItem == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No pending item found with given Id"
                };
            }
            // TODO add item to returned items
            var existingExchange = await _db.ItemExchangeRequests
                .FirstOrDefaultAsync(ie => ie.Id == pendingItem.ItemExchangeRequestId);

            var existingItemAtReturns = await _db.returnedItemsFromCustomers
                .FirstOrDefaultAsync(ri => ri.OrderId == existingExchange.OrderId
                && ri.ProductVariantId == pendingItem.ReturnedProductVariantId);
            if(existingItemAtReturns == null)
            {
                var itemAtReturns = new ReturnedItemsFromCustomer()
                {
                    OrderId = existingExchange.OrderId,
                    BaseProductId = pendingItem.BaseProductId,
                    BaseProductName = pendingItem.BaseProductName,
                    ProductVariantId = pendingItem.ReturnedProductVariantId,
                    ProductVariantColor = pendingItem.ReturnedProductVariantColor,
                    ProductVariantSize = pendingItem.ReturnedProductVariantSize,
                    PricePerItem = pendingItem.PricePerItemPaid,
                    Quantity = exchangePendingItem.Quantity
                };

                await _db.returnedItemsFromCustomers.AddAsync(itemAtReturns);
                await _db.SaveChangesAsync();
            }
            else
            {
                existingItemAtReturns.Quantity = existingItemAtReturns.Quantity
                    + exchangePendingItem.Quantity;

                await _db.SaveChangesAsync();
            }
            // TODO remove item from pending items qty if == 0, remove item
            pendingItem.Quantity = pendingItem.Quantity - exchangePendingItem.Quantity;
            if(pendingItem.Quantity == 0)
            {
                _db.ExchangeItemsPending.Remove(pendingItem);
            }
            await _db.SaveChangesAsync();

            return new Response_IsSuccess()
            {
                isSuccess = true,
                Message = "Pending item removed successfully"
            };

        }

        public async Task<Response_IsSuccess> MovePendingItemToGood(Request_MovePendingToGood movePendingToGood)
        {
            //TODO check if pending item exist
            var pendingItem = await _db.ExchangeItemsPending
                .FirstOrDefaultAsync(ei => ei.Id == movePendingToGood.PendingItemId);
            if(pendingItem == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No pending item exist with given pending item Id"
                };
            }
            //TODO check if product variant Exist
            var productVariant = await _db.ProductVariants
                .Include(pv => pv.productSize)
                .Include(pv => pv.productColor)
                .FirstOrDefaultAsync(pv => pv.Id == movePendingToGood.ExchangeProductVariantId);
            if(productVariant == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No product variant to exchange exist with given Id"
                };
            }
            //TODO check if where is enough qty for pendingItem
            if(pendingItem.Quantity < movePendingToGood.Quantity)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Not enough quantity in pending item to exchange"
                };
            }
            //TODO check if where is enough qty for exchangeItem 
            if(productVariant.Quantity < movePendingToGood.Quantity)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Not enough quanity in stock, to exchange"
                };
            }
            //TODO check if pending item and product variant have same BaseProductId
            if(productVariant.BaseProductId != pendingItem.BaseProductId)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Product to exchange and pending items is not the same base product"
                };
            }
            //TODO remove qty from pendingItem if == 0 remove pendingItem
            pendingItem.Quantity = pendingItem.Quantity - movePendingToGood.Quantity;
            if(pendingItem.Quantity ==  0)
            {
                _db.ExchangeItemsPending.Remove(pendingItem);
            }
            await _db.SaveChangesAsync();
            //TODO remove qty from productVariant
            await RemoveItemQuantity(productVariant.Id, movePendingToGood.Quantity);
            //TODO check if exchange good item exist add qty or add exchangeGoodItem
            var existingExchangeOrderItem = await _db.ExchangeOrderItems
                .FirstOrDefaultAsync(eo => eo.ItemExchangeRequestId ==
                pendingItem.ItemExchangeRequestId
                && eo.ReturnedProductVariantId == pendingItem.ReturnedProductVariantId
                && eo.ExchangedProductVariantId == productVariant.Id);
            if(existingExchangeOrderItem == null)
            {
                var exchangeOrderItem = new ExchangeOrderItem();
                exchangeOrderItem.ItemExchangeRequestId = pendingItem.ItemExchangeRequestId;
                exchangeOrderItem.BaseProductId = pendingItem.BaseProductId;
                exchangeOrderItem.BaseProductName = pendingItem.BaseProductName;

                exchangeOrderItem.ReturnedProductVariantId = 
                    pendingItem.ReturnedProductVariantId;

                exchangeOrderItem.ReturnedProductVariantColor =
                    pendingItem.ReturnedProductVariantColor;

                exchangeOrderItem.ReturnedProductVariantSize =
                    pendingItem.ReturnedProductVariantSize;

                exchangeOrderItem.ExchangedProductVariantId = productVariant.Id;

                exchangeOrderItem.ExchangedProductVariantColor =
                    productVariant.productColor.Name;

                exchangeOrderItem.ExchangedProductVariantSize =
                    productVariant.productSize.Name;

                exchangeOrderItem.PricePerItemPaid = pendingItem.PricePerItemPaid;
                exchangeOrderItem.Quantity = movePendingToGood.Quantity;
                exchangeOrderItem.Message = "Items good for exchange";

                await _db.ExchangeOrderItems.AddAsync(exchangeOrderItem);
                await _db.SaveChangesAsync();
            }
            else
            {
                existingExchangeOrderItem.Quantity = existingExchangeOrderItem.Quantity
                    + movePendingToGood.Quantity;

                await _db.SaveChangesAsync();
            }


            return new Response_IsSuccess()
            {
                isSuccess = true,
                Message = "Item moved to exchange items"
            };

        }

        public async Task<Response_IsSuccess> MovePendingItemToBad(Request_MovePendingToBad movePendingToBad)
        {
            //TODO check if pending item exist
            var pendingItem = await _db.ExchangeItemsPending
                .FirstOrDefaultAsync(ei => ei.Id == movePendingToBad.PendingItemId);
            if(pendingItem == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No pending item exist with given Id"
                };
            }
            //TODO check if pending item have enough quantity
            if(pendingItem.Quantity < movePendingToBad.Quantity)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Not enough quantity in pending items"
                };
            }
            //TODO check if item exist in canceled items add qty or item to badItems
            var existingCanceledItem = await _db.ExchangeItemsCanceled
                .FirstOrDefaultAsync(ei => ei.ItemExchangeRequestId == pendingItem.ItemExchangeRequestId
                && ei.ReturnedProductVariantId == pendingItem.ReturnedProductVariantId);
            if(existingCanceledItem == null)
            {
                var exchangeItemCanceled = new ExchangeItemCanceled();
                exchangeItemCanceled.ItemExchangeRequestId = pendingItem.ItemExchangeRequestId;
                exchangeItemCanceled.BaseProductId = pendingItem.BaseProductId;
                exchangeItemCanceled.BaseProductName = pendingItem.BaseProductName;
                exchangeItemCanceled.PricePerItemPaid = pendingItem.PricePerItemPaid;

                exchangeItemCanceled.ReturnedProductVariantId = 
                    pendingItem.ReturnedProductVariantId;

                exchangeItemCanceled.ReturnedProductVariantSize = 
                    pendingItem.ReturnedProductVariantSize;

                exchangeItemCanceled.ReturnedProductVariantColor =
                    pendingItem.ReturnedProductVariantColor;

                exchangeItemCanceled.Quantity = movePendingToBad.Quantity;
                exchangeItemCanceled.CancelationReason = movePendingToBad.Message;

                await _db.ExchangeItemsCanceled.AddAsync(exchangeItemCanceled);

                await _db.SaveChangesAsync();
            }
            else
            {
                existingCanceledItem.Quantity = existingCanceledItem.Quantity +
                    movePendingToBad.Quantity;
                existingCanceledItem.CancelationReason = movePendingToBad.Message;

                await _db.SaveChangesAsync();
            }
            //TODO remove qty from pending item, if  quantity == 0 , remove pending item
            pendingItem.Quantity = pendingItem.Quantity - movePendingToBad.Quantity;
            if(pendingItem.Quantity  == 0)
            {
                _db.ExchangeItemsPending.Remove(pendingItem);
            }
            await _db.SaveChangesAsync();

            return new Response_IsSuccess()
            {
                isSuccess = true,
                Message = "Items moved to canceled"
            };
        }

        public async Task<Response_AllExchangeBadItems> GetAllExchangeBadItems(string exchangeUniqueIdentifier)
        {
            //TODO get exchange request, include exchange bad items
            var exchangeOrder = await  _db.ItemExchangeRequests
                .Include(ie=>ie.exchangeItemsCanceled)
                .FirstOrDefaultAsync(ie=>ie.ExchangeUniqueIdentifier ==  exchangeUniqueIdentifier);
            //TODO check if exchange exist
            if(exchangeOrder == null)
            {
                return new Response_AllExchangeBadItems()
                {
                    isSuccess = false,
                    Message = "No exchange order found with given Exchange Unique ID"
                };
            }
            //TODO convertToDTO item
            var allExchangeBadItemsResponse = exchangeOrder.ConvertToDtoBadItems();
            allExchangeBadItemsResponse.isSuccess = true;
            allExchangeBadItemsResponse.Message = "All items not suitable for exchange";

            return allExchangeBadItemsResponse;

        }

        public async Task<Response_IsSuccess> AddExchangeBadItem(Request_AddExchangeBadItem addExchangeBadItem)
        {
            //TODO check if exchange order exist
            var existingExchange = await _db.ItemExchangeRequests
                .FirstOrDefaultAsync(ie=>ie.ExchangeUniqueIdentifier ==
                addExchangeBadItem.ExchangeUniqueIdentifier);
            if(existingExchange == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No exchange order found with given Exchange Unique ID"
                };
            }
            //TODO check if exchange order is closed
            if(existingExchange.RequestClosed == true)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Exchange order is closed , and cant be modified"
                };
            }
            //TODO check if order exist
            var existingOrder = await _db.Orders
                .Include(o => o.ReturnedItemsFromCustomers)
                .FirstOrDefaultAsync(o => o.OrderId == existingExchange.OrderId);

            if(existingOrder == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No order found"
                };
            }
            //TODO check if items exist at ReturnedItemsFromCustomer
            //TODO check if customer have enough quantity
            bool itemExistInReturns = false;
            bool enoughQty = false;
            int existingItemAtReturnsId = 0;

            foreach(var item in existingOrder.ReturnedItemsFromCustomers)
            {
                if(item.ProductVariantId == addExchangeBadItem.ReturnedProductVariantId)
                {
                    itemExistInReturns = true;
                    existingItemAtReturnsId = item.Id;
                    if(item.Quantity >= addExchangeBadItem.Quantity)
                    {
                        enoughQty = true;
                    }
                }
            }

            if(itemExistInReturns == false)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No item at returns with given product variant ID"
                };
            }
            if(enoughQty == false)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Not enough items in returned items to exchange"
                };
            }

            //TODO check if productVariant already exist in badItems and add accordingly
            var existingBadItemAtCustomer = await _db.ExchangeItemsCanceled
                .FirstOrDefaultAsync(ei => ei.ItemExchangeRequestId == existingExchange.Id
                && ei.ReturnedProductVariantId == addExchangeBadItem.ReturnedProductVariantId);
            if(existingBadItemAtCustomer == null)
            {
                var existingItemAtReturns = await _db.returnedItemsFromCustomers
                    .FirstOrDefaultAsync(ri => ri.OrderId == existingExchange.OrderId
                    && ri.ProductVariantId == addExchangeBadItem.ReturnedProductVariantId);

                var canceledItem = new ExchangeItemCanceled()
                {
                    ItemExchangeRequestId = existingExchange.Id,
                    BaseProductId = existingItemAtReturns.BaseProductId,
                    BaseProductName = existingItemAtReturns.BaseProductName,
                    ReturnedProductVariantId = existingItemAtReturns.ProductVariantId,
                    ReturnedProductVariantColor = existingItemAtReturns.ProductVariantColor,
                    ReturnedProductVariantSize = existingItemAtReturns.ProductVariantSize,
                    PricePerItemPaid = existingItemAtReturns.PricePerItem,
                    Quantity = addExchangeBadItem.Quantity,
                    CancelationReason = addExchangeBadItem.Message
                };

                await _db.ExchangeItemsCanceled.AddAsync(canceledItem);
                await _db.SaveChangesAsync();
            }
            else
            {
                existingBadItemAtCustomer.Quantity = existingBadItemAtCustomer.Quantity
                    + addExchangeBadItem.Quantity;

                await _db.SaveChangesAsync();
            }

            //TODO Remove quantity from returned items from customer, if qty == 0 remove itemAtCustomer
            var itemInReturns = await _db.returnedItemsFromCustomers
                .FirstOrDefaultAsync(ri => ri.Id == existingItemAtReturnsId);

            itemInReturns.Quantity = itemInReturns.Quantity - addExchangeBadItem.Quantity;
            if(itemInReturns.Quantity == 0)
            {
                _db.returnedItemsFromCustomers.Remove(itemInReturns);
            }

            await _db.SaveChangesAsync();

            return new Response_IsSuccess()
            {
                isSuccess = true,
                Message = "Item added to bad items"
            };

        }

        public async Task<Response_IsSuccess> RemoveExchangeBadItem(Request_RemoveExchangeBadItem removeExchangeBadItem)
        {
            //TODO check if exchange Canceled Item exist
            var existingCanceledItem = await _db.ExchangeItemsCanceled
                .FirstOrDefaultAsync(ei => ei.Id == removeExchangeBadItem.ExchangeItemId);
            if(existingCanceledItem == null)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "No canceled items found with given Id"
                };
            }
            //TODO check if exchange Canceled Item has enough qty
            if(existingCanceledItem.Quantity < removeExchangeBadItem.Quantity)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Not enough items at canceled Items"
                };
            }
            //TODO get existing Exchange Order
            var existingExchangeOrder = await _db.ItemExchangeRequests
                .FirstOrDefaultAsync(ie => ie.Id == existingCanceledItem.ItemExchangeRequestId);
            //TODO check if exchange order is closed
            if(existingExchangeOrder.RequestClosed == true)
            {
                return new Response_IsSuccess()
                {
                    isSuccess = false,
                    Message = "Exchange request is closed and can not be modified"
                };
            }
            //TODO check if itemReturnedFromCustomer exist, if exist add qty if not create new returnedItem
            var existingItemInReturns = await _db.returnedItemsFromCustomers
                .FirstOrDefaultAsync(ri => ri.OrderId == existingExchangeOrder.OrderId
                && ri.ProductVariantId == existingCanceledItem.ReturnedProductVariantId);
            if(existingItemInReturns == null)
            {
                var returnedItemFromCustomer = new ReturnedItemsFromCustomer()
                {
                    OrderId = existingExchangeOrder.OrderId,
                    BaseProductId = existingCanceledItem.BaseProductId,
                    BaseProductName = existingCanceledItem.BaseProductName,
                    ProductVariantId = existingCanceledItem.ReturnedProductVariantId,
                    ProductVariantColor = existingCanceledItem.ReturnedProductVariantColor,
                    ProductVariantSize = existingCanceledItem.ReturnedProductVariantSize,
                    PricePerItem = existingCanceledItem.PricePerItemPaid,
                    Quantity = removeExchangeBadItem.Quantity
                };

                await _db.returnedItemsFromCustomers.AddAsync(returnedItemFromCustomer);
                await _db.SaveChangesAsync();

            }
            else
            {
                existingItemInReturns.Quantity = existingItemInReturns.Quantity
                    + removeExchangeBadItem.Quantity;

                await _db.SaveChangesAsync();
            }

            //TODO remove qty from canceledItem if qty == 0 remove exchange Canceled Item
            existingCanceledItem.Quantity = existingCanceledItem.Quantity
                - removeExchangeBadItem.Quantity;
            if(existingCanceledItem.Quantity == 0)
            {
                _db.ExchangeItemsCanceled.Remove(existingCanceledItem);
            }

            await _db.SaveChangesAsync();

            return new Response_IsSuccess()
            {
                isSuccess = true,
                Message = "Item(s) removed from exchange bad items"
            };

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

        private async Task<bool> RemoveItemQuantity(int productVariantId, int quantity)
        {
            var existingProductVariant = await _db.ProductVariants
                .FirstOrDefaultAsync(pv=>pv.Id == productVariantId);
            existingProductVariant.Quantity = existingProductVariant.Quantity - quantity;
            await _db.SaveChangesAsync();

            return true;
        }
        private async Task<bool> AddItemQuantity(int productVariantId, int quanity)
        {
            var existingProductVariant = await _db.ProductVariants
                .FirstOrDefaultAsync(pv=>pv.Id ==productVariantId);
            existingProductVariant.Quantity = existingProductVariant.Quantity + quanity;

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<string> CreateExchangePdf(ItemExchangeRequest itemExchangeRequest, string userId)
        {
            var date = DateTime.Now.ToShortDateString().ToString();
            var dateNormilized = date.Replace("/", "_");
            string fileName = "PDF_" + dateNormilized + "_" + DateTime.UtcNow.Millisecond + ".pdf";

            string folderPath = _webHostEnvironment.WebRootPath + $"\\PDF\\{userId}\\{itemExchangeRequest.OrderUniqueIdentifier}";
            string path = System.IO.Path.Combine(folderPath, fileName);

            decimal TotalDiscountOfAllItems = 0;
            decimal TotalPriceOfAllItems = 0;

            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(path));
            iText.Layout.Document doc = new iText.Layout.Document(pdfDoc, PageSize.A4, true);
            doc.SetMargins(25, 25, 25, 25);

            var font = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.TIMES_ROMAN);
            var aligmentLeft = iText.Layout.Properties.TextAlignment.LEFT;
            var aligmentCenter = iText.Layout.Properties.TextAlignment.CENTER;
            string imagePath = _webHostEnvironment.WebRootPath + "\\ImageForPDF\\Logo.png";
            iText.Layout.Element.Image image = new iText.Layout.Element.Image
                (ImageDataFactory.Create(imagePath));
            image.SetFixedPosition(400, 700);
            image.ScaleToFit(90, 90);
            doc.Add(image);

            //customer info

            Paragraph name = new Paragraph(itemExchangeRequest.UserFirstName + " " + itemExchangeRequest.UserLastName)
                .SetFont(font)
                .SetFontSize(12)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
            doc.Add(name);

            Paragraph email = new Paragraph(itemExchangeRequest.UserEmail)
                .SetFont(font)
                .SetFontSize(14)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
            doc.Add(email);

            Paragraph street = new Paragraph(itemExchangeRequest.Street)
                .SetFont(font)
                .SetFontSize(14)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
            doc.Add(street);

            if (itemExchangeRequest.ApartmentNumber == null || itemExchangeRequest.ApartmentNumber == 0)
            {
                Paragraph addressHouseNum = new Paragraph(itemExchangeRequest.HouseNumber.ToString())
                .SetFont(font)
                .SetFontSize(14)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
                doc.Add(addressHouseNum);
            }
            else
            {
                Paragraph addressHouseApp = new Paragraph(itemExchangeRequest.HouseNumber.ToString() + "-" + itemExchangeRequest.ApartmentNumber.ToString())
                .SetFont(font)
                .SetFontSize(14)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
                doc.Add(addressHouseApp);
            }

            Paragraph addressCity = new Paragraph(itemExchangeRequest.City)
                .SetFont(font)
                .SetFontSize(14)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
            doc.Add(addressCity);

            Paragraph addressRegion = new Paragraph(itemExchangeRequest.Region)
                .SetFont(font)
                .SetFontSize(14)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
            doc.Add(addressRegion);

            Paragraph addressCountry = new Paragraph(itemExchangeRequest.Country)
                .SetFont(font)
                .SetFontSize(14)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
            doc.Add(addressCountry);

            Paragraph postalCode = new Paragraph(itemExchangeRequest.PostalCode)
                .SetFont(font)
                .SetFontSize(14)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
            doc.Add(postalCode);

            Paragraph busineesName = new Paragraph("My Bussines Name")
               .SetFont(font)
               .SetFontSize(12)
               .SetTextAlignment(aligmentCenter)
               .SetPaddingTop(20);
            doc.Add(busineesName);

            Paragraph orderId = new Paragraph("Invoice for order: " + itemExchangeRequest.OrderUniqueIdentifier)
               .SetFont(font)
               .SetFontSize(12)
               .SetTextAlignment(aligmentCenter)
               .SetPaddingTop(5);
            doc.Add(orderId);

            if (itemExchangeRequest.exchangeOrderItems.Count > 0)
            {
                Table ExchangeGoodTable = new Table(6);
                ExchangeGoodTable.SetMarginTop(20);
                ExchangeGoodTable.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                ExchangeGoodTable.SetWidth(500);

                //iText.Kernel.Colors
                Color textColorTableHeadings = new DeviceRgb(255, 255, 255);
                Color bgColorTableHeadings = new DeviceRgb(1, 1, 1);

                Cell cell1 = new Cell().Add(new Paragraph("PRODUCT NAME")
                .SetFontColor(textColorTableHeadings)
                .SetFontSize(8)
                .SetTextAlignment(aligmentCenter));
                cell1.SetBackgroundColor(bgColorTableHeadings);
                cell1.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ExchangeGoodTable.AddCell(cell1);

                Cell cell2 = new Cell().Add(new Paragraph("RETURNED PRODUCT COLOR")
                .SetFontColor(textColorTableHeadings)
                .SetFontSize(8)
                .SetTextAlignment(aligmentCenter));
                cell2.SetBackgroundColor(bgColorTableHeadings);
                cell2.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ExchangeGoodTable.AddCell(cell2);

                Cell cell3 = new Cell().Add(new Paragraph("RETURNED PRODUCT SIZE")
                .SetFontColor(textColorTableHeadings)
                .SetFontSize(8)
                .SetTextAlignment(aligmentCenter));
                cell3.SetBackgroundColor(bgColorTableHeadings);
                cell3.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ExchangeGoodTable.AddCell(cell3);

                Cell cell4 = new Cell().Add(new Paragraph("EXCHANGE PRODUCT COLOR")
                .SetFontColor(textColorTableHeadings)
                .SetFontSize(8)
                .SetTextAlignment(aligmentCenter));
                cell4.SetBackgroundColor(bgColorTableHeadings);
                cell4.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ExchangeGoodTable.AddCell(cell4);

                Cell cell5 = new Cell().Add(new Paragraph("EXCHANGE PRODUCT SIZE")
                .SetFontColor(textColorTableHeadings)
                .SetFontSize(8)
                .SetTextAlignment(aligmentCenter));
                cell5.SetBackgroundColor(bgColorTableHeadings);
                cell5.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ExchangeGoodTable.AddCell(cell5);

                Cell cell6 = new Cell().Add(new Paragraph("QUANTITY")
                .SetFontColor(textColorTableHeadings)
                .SetFontSize(8)
                .SetTextAlignment(aligmentCenter));
                cell6.SetBackgroundColor(bgColorTableHeadings);
                cell6.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ExchangeGoodTable.AddCell(cell6);

                foreach (var item in itemExchangeRequest.exchangeOrderItems)
                {
                    Cell CellProductName = new Cell().Add(new Paragraph(item.BaseProductName)
                    .SetTextAlignment(aligmentCenter)
                    .SetFontSize(8));
                    CellProductName.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                    ExchangeGoodTable.AddCell(CellProductName);

                    Cell ReturnedProductColor = new Cell().Add(new Paragraph(item.ReturnedProductVariantColor)
                    .SetTextAlignment(aligmentCenter)
                    .SetFontSize(8));
                    ReturnedProductColor.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                    ExchangeGoodTable.AddCell(ReturnedProductColor);

                    Cell ReturnedProductSize = new Cell().Add(new Paragraph(item.ReturnedProductVariantSize)
                   .SetTextAlignment(aligmentCenter)
                   .SetFontSize(8));
                    ReturnedProductSize.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                    ExchangeGoodTable.AddCell(ReturnedProductSize);

                    Cell ExchangedProductColor = new Cell().Add(new Paragraph(item.ExchangedProductVariantColor)
                    .SetTextAlignment(aligmentCenter)
                    .SetFontSize(8));
                    ExchangedProductColor.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                    ExchangeGoodTable.AddCell(ExchangedProductColor);

                    Cell ExchangedProductSize = new Cell().Add(new Paragraph(item.ExchangedProductVariantSize)
                   .SetTextAlignment(aligmentCenter)
                   .SetFontSize(8));
                    ExchangedProductSize.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                    ExchangeGoodTable.AddCell(ExchangedProductSize);

                    Cell Quantity = new Cell().Add(new Paragraph(item.Quantity.ToString())
                   .SetTextAlignment(aligmentCenter)
                   .SetFontSize(8));
                    Quantity.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                    ExchangeGoodTable.AddCell(Quantity);
                }
                doc.Add(ExchangeGoodTable);
            }

            //returned product which wont be refunded

            if (itemExchangeRequest.exchangeItemsCanceled.Count > 0)
            {
                Table ExchangeBadTable = new Table(5);
                ExchangeBadTable.SetMarginTop(20);
                ExchangeBadTable.SetHorizontalAlignment(HorizontalAlignment.CENTER);
                ExchangeBadTable.SetWidth(500);

                //iText.Kernel.Colors
                Color textColorTableHeadings = new DeviceRgb(255, 255, 255);
                Color bgColorTableHeadings = new DeviceRgb(1, 1, 1);

                Cell cell1 = new Cell().Add(new Paragraph("PRODUCT NAME")
                .SetFontColor(textColorTableHeadings)
                .SetFontSize(8)
                .SetTextAlignment(aligmentCenter));
                cell1.SetBackgroundColor(bgColorTableHeadings);
                cell1.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ExchangeBadTable.AddCell(cell1);

                Cell cell2 = new Cell().Add(new Paragraph("RETURNED PRODUCT COLOR")
                .SetFontColor(textColorTableHeadings)
                .SetFontSize(8)
                .SetTextAlignment(aligmentCenter));
                cell2.SetBackgroundColor(bgColorTableHeadings);
                cell2.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ExchangeBadTable.AddCell(cell2);

                Cell cell3 = new Cell().Add(new Paragraph("RETURNED PRODUCT SIZE")
                .SetFontColor(textColorTableHeadings)
                .SetFontSize(8)
                .SetTextAlignment(aligmentCenter));
                cell3.SetBackgroundColor(bgColorTableHeadings);
                cell3.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ExchangeBadTable.AddCell(cell3);

                Cell cell4 = new Cell().Add(new Paragraph("QUANTITY")
                .SetFontColor(textColorTableHeadings)
                .SetFontSize(8)
                .SetTextAlignment(aligmentCenter));
                cell4.SetBackgroundColor(bgColorTableHeadings);
                cell4.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ExchangeBadTable.AddCell(cell4);

                Cell cell5 = new Cell().Add(new Paragraph("NO EXCHANGE REASON")
                .SetFontColor(textColorTableHeadings)
                .SetFontSize(8)
                .SetTextAlignment(aligmentCenter));
                cell5.SetBackgroundColor(bgColorTableHeadings);
                cell5.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ExchangeBadTable.AddCell(cell5);

                foreach (var item in itemExchangeRequest.exchangeItemsCanceled)
                {
                    Cell CellProductName = new Cell().Add(new Paragraph(item.BaseProductName)
                    .SetTextAlignment(aligmentCenter)
                    .SetFontSize(8));
                    CellProductName.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                    ExchangeBadTable.AddCell(CellProductName);

                    Cell ReturnedProductColor = new Cell().Add(new Paragraph(item.ReturnedProductVariantColor)
                    .SetTextAlignment(aligmentCenter)
                    .SetFontSize(8));
                    ReturnedProductColor.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                    ExchangeBadTable.AddCell(ReturnedProductColor);

                    Cell ReturnedProductSize = new Cell().Add(new Paragraph(item.ReturnedProductVariantSize)
                   .SetTextAlignment(aligmentCenter)
                   .SetFontSize(8));
                    ReturnedProductSize.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                    ExchangeBadTable.AddCell(ReturnedProductSize);

                    Cell Quantity = new Cell().Add(new Paragraph(item.Quantity.ToString())
                   .SetTextAlignment(aligmentCenter)
                   .SetFontSize(8));
                    Quantity.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                    ExchangeBadTable.AddCell(Quantity);

                    Cell ReasonNoExchange = new Cell().Add(new Paragraph(item.CancelationReason)
                    .SetTextAlignment(aligmentCenter)
                    .SetFontSize(8));
                    ReasonNoExchange.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                    ExchangeBadTable.AddCell(ReasonNoExchange);
                }

                doc.Add(ExchangeBadTable);
            }

            doc.Close();

            return path;
        }
        #endregion

    }
}
