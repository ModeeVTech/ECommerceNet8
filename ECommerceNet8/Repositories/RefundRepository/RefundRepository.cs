using ECommerceNet8.Data;
using ECommerceNet8.DTOs.RefundRequestDtos.Request;
using ECommerceNet8.DTOs.RefundRequestDtos.Response;
using ECommerceNet8.Models.OrderModels;
using ECommerceNet8.Templates;
using Microsoft.EntityFrameworkCore;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ECommerceNet8.Repositories.RefundRepository
{
    public class RefundRepository : IRefundRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly ISendGridClient _sendGridClient;

        public RefundRepository(ApplicationDbContext db,
            IConfiguration configuration,
            ISendGridClient sendGridClient)
        {
            _db = db;
            _configuration = configuration;
            _sendGridClient = sendGridClient;
        }


        public async Task<Response_Refund> CreateRefundOrder(Request_Refund refundRequest)
        {
            var existingRefundOrder = await _db.ItemReturnRequests
                .FirstOrDefaultAsync(ir=>ir.ExchangeUniqueIdentifier 
                == refundRequest.ExchangeUniqueIdentifier);

            if (existingRefundOrder !=null)
            {
                return new Response_Refund
                {
                    isSuccess = false,
                    OrderUniqueIdentifier = refundRequest.OrderUniqueIdentifer,
                    ExchangeUniqueIdentifier = refundRequest.ExchangeUniqueIdentifier,
                    Message = "Refund already exist"
                };
            }

            var existingOrder = await _db.Orders
                .FirstOrDefaultAsync(o => o.OrderUniqueIdentifier 
                == refundRequest.OrderUniqueIdentifer);

            if (existingOrder == null)
            {
                return new Response_Refund()
                {
                    isSuccess = false,
                    OrderUniqueIdentifier = refundRequest.OrderUniqueIdentifer,
                    ExchangeUniqueIdentifier = refundRequest.ExchangeUniqueIdentifier,
                    Message = "No order found with given order unique id"
                };
            }

            var itemReturnRequest = new ItemReturnRequest()
            {
                RequestClosed = false,
                RequestRefunded = false,
                ExchangeRequestTime = refundRequest.ExchangeRequestTime,
                OrderId = existingOrder.OrderId,
                OrderUniqueIdentifier = existingOrder.OrderUniqueIdentifier,
                ExchangeUniqueIdentifier = refundRequest.ExchangeUniqueIdentifier,
                AdminId = refundRequest.AdminId,
                AdminFullName = refundRequest.AdminName,
                UserEmail = refundRequest.Email,
                UserPhone = refundRequest.PhoneNumber,
                UserBankName = refundRequest.BankName,
                UserBankAccount = refundRequest.AccountNumber,
                totalAmountNotRefunded = 0,
                totalAmountRefunded = 0,
                totalRequestForRefund = 0,
            };

            await _db.ItemReturnRequests.AddAsync(itemReturnRequest);
            await _db.SaveChangesAsync();

            return new Response_Refund()
            {
                isSuccess = true,
                OrderUniqueIdentifier = itemReturnRequest.OrderUniqueIdentifier,
                ExchangeUniqueIdentifier = itemReturnRequest.ExchangeUniqueIdentifier,
                ReturnRequestId = itemReturnRequest.Id,
                Message = "Refund Request Created Successfully"
            };

        }

        public async Task<Response_RefundFullInfo> GetRefundRequest(string exchangeUniqueIdentifier)
        {
           var refundRequest = await _db.ItemReturnRequests
                .Include(ir=>ir.itemsGoodForRefund)
                .Include(ir=>ir.itemsBadForRefund)
                .FirstOrDefaultAsync(ir=>ir.ExchangeUniqueIdentifier 
                == exchangeUniqueIdentifier);

            if(refundRequest == null)
            {
                return new Response_RefundFullInfo()
                {
                    isSuccess = false,
                    Message = "No refund request found with given exchange unique identifier"
                };
            }


            return new Response_RefundFullInfo()
            {
                isSuccess = true,
                OrderUniqueIdentifier = refundRequest.OrderUniqueIdentifier,
                ExchangeUniqueIdentifier = refundRequest.ExchangeUniqueIdentifier,
                Id = refundRequest.Id,
                OrderId = refundRequest.OrderId,
                AdminId = refundRequest.AdminId,
                AdminFullName = refundRequest.AdminFullName,
                UserEmail = refundRequest.UserEmail,
                UserPhone = refundRequest.UserPhone,
                UserBankName = refundRequest.UserBankName,
                UserBankAccount = refundRequest.UserBankAccount,
                ExchangeRequestTime = refundRequest.ExchangeRequestTime,
                ItemsGoodForRefund = refundRequest.itemsGoodForRefund,
                ItemsBadForRefund = refundRequest.itemsBadForRefund,
                TotalRequestForRefund = refundRequest.totalRequestForRefund,
                TotalAmountRefunded = refundRequest.totalAmountRefunded,
                TotalAmountNotRefunded = refundRequest.totalAmountNotRefunded,

                RequestRefunded = refundRequest.RequestRefunded,
                RequestClosed = refundRequest.RequestClosed
               
            };

        }
        public async Task<Response_RefundIsSuccess> AddReturnedGoodItem(Request_AddGoodRefundItem addGoodRefundItem)
        {
            //TODO Check if return order Exist
            var existingReturnRequest = await _db.ItemReturnRequests
                .Include(ir => ir.itemsGoodForRefund)
                .FirstOrDefaultAsync(ir => ir.ExchangeUniqueIdentifier
                == addGoodRefundItem.ExchangeUniqueIdentifier);

            if(existingReturnRequest == null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No return request exist with given Exchange Unique Id"
                };
            }
            //TODO Get The Order with items at customer
            var existingOrder  = await _db.Orders
                .Include(o=>o.ItemsAtCustomer)
                .FirstOrDefaultAsync(o=>o.OrderUniqueIdentifier
                == existingReturnRequest.OrderUniqueIdentifier);

            if(existingOrder == null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No Order exist for this Return Request"
                };
            }

            //TODO check if Product Variant exist
            var existingProductVariant = await _db.ProductVariants
                .FirstOrDefaultAsync(pv => pv.Id == addGoodRefundItem.ProductVariantId);

            if(existingProductVariant == null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No product variant exist with given Id"
                };
            }
            //TODO check if Returned Items exist and has enough Quantity
            var existingReturnItem = await _db.returnedItemsFromCustomers
                .FirstOrDefaultAsync(ri=>ri.OrderId == existingOrder.OrderId
                && ri.ProductVariantId== existingProductVariant.Id);

            if(existingReturnItem == null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No product variant exist in returned items"
                };
            }

            var itemAtCustomerPricePaid = existingReturnItem.PricePerItem;

            if(existingReturnItem.Quantity < addGoodRefundItem.Quantity)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = $"Customer has returned {existingReturnItem.Quantity.ToString()}" +
                    $" items, but requested to exchange {addGoodRefundItem.Quantity.ToString()} items"
                };
            }

            //TODO Check if item good for refund already exist with productVariantID and deal with quantity accordingly

            var itemExistInGoodItemsForRefund = false;
            int goodReturnItemId = 0;

            foreach(var item in existingReturnRequest.itemsGoodForRefund)
            {
                if(item.ProductVariantId == existingProductVariant.Id)
                {
                    itemExistInGoodItemsForRefund = true;
                    goodReturnItemId = item.Id;
                }
            }

            if(itemExistInGoodItemsForRefund == true)
            {
                var existingGoodReturnItem = await _db.ItemsGoodForRefund
                    .FirstOrDefaultAsync(ig => ig.Id == goodReturnItemId);

                existingGoodReturnItem.Quantity = existingGoodReturnItem.Quantity
                    + addGoodRefundItem.Quantity;

                await _db.SaveChangesAsync();
            }
            else
            {
                var itemGoodForRefund = new ItemGoodForRefund()
                {
                    ItemReturnRequestId = existingReturnRequest.Id,
                    BaseProductId = existingReturnItem.BaseProductId,
                    BaseProductName = existingReturnItem.BaseProductName,
                    ProductVariantId = existingReturnItem.ProductVariantId,
                    ProductColor = existingReturnItem.ProductVariantColor,
                    ProductSize = existingReturnItem.ProductVariantSize,
                    PricePaidPerItem = itemAtCustomerPricePaid,
                    Quantity = addGoodRefundItem.Quantity
                };

                await _db.ItemsGoodForRefund.AddAsync(itemGoodForRefund);
                await _db.SaveChangesAsync();
            }


            //TODO remove quantity from returned items if quantity == 0 delete returnedItem
            existingReturnItem.Quantity = existingReturnItem.Quantity 
                - addGoodRefundItem.Quantity;
            if(existingReturnItem.Quantity ==  0)
            {
                _db.returnedItemsFromCustomers.Remove(existingReturnItem);
            }

            await _db.SaveChangesAsync();

            //TODO deal with exchange refund amounts

            existingReturnRequest.totalRequestForRefund =
                existingReturnRequest.totalRequestForRefund +
                (itemAtCustomerPricePaid * addGoodRefundItem.Quantity);

            existingReturnRequest.totalAmountRefunded +=
                (itemAtCustomerPricePaid * addGoodRefundItem.Quantity);

            await  _db.SaveChangesAsync();

            return new Response_RefundIsSuccess()
            {
                isSuccess = true,
                Message = "Item Added To Good Return Items"
            };

        }


        public async Task<Response_RefundIsSuccess> CancelGoodReturnedItem(Request_CancelRefundItem cancelRefundItem)
        {
            //TODO Check if item exist in Refund Good Items
            var existingReturnedGoodItem = await _db.ItemsGoodForRefund
                .FirstOrDefaultAsync(ig => ig.Id == cancelRefundItem.ReturnItemId);

            if(existingReturnedGoodItem ==  null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No existing good returned items found"
                };
            }
            //TODO check if Return Request Exist
            var existingReturnRequest = await _db.ItemReturnRequests
                .FirstOrDefaultAsync(ir => ir.Id 
                == existingReturnedGoodItem.ItemReturnRequestId);

            if(existingReturnRequest == null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No existing Return Request found"
                };
            }
            //Get Order with Returned Items From Customer
            var existingOrder = await _db.Orders
                .Include(o=>o.ReturnedItemsFromCustomers)
                .FirstOrDefaultAsync(o=>o.OrderId == existingReturnRequest.OrderId);

            if(existingOrder  == null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No order exist with given orderId"
                };
            }
            //TODO Add quantity to returned items or add returned item
            bool itemIsInReturnedItems = false;
            foreach(var item in existingOrder.ReturnedItemsFromCustomers)
            {
                if(item.ProductVariantId == existingReturnedGoodItem.ProductVariantId)
                {
                    itemIsInReturnedItems = true;
                }
            }

            if(itemIsInReturnedItems  == true)
            {
                foreach(var item in existingOrder.ReturnedItemsFromCustomers)
                {
                    if(item.ProductVariantId == existingReturnedGoodItem.ProductVariantId)
                    {
                        var returnedItemAtCustomer = await _db.returnedItemsFromCustomers
                            .FirstOrDefaultAsync(ri => ri.Id == item.Id);

                        returnedItemAtCustomer.Quantity =
                            returnedItemAtCustomer.Quantity + cancelRefundItem.Quantity;

                        await _db.SaveChangesAsync();
                    }
                }
            }
            else
            {
                var returnItemFromCustomer = new ReturnedItemsFromCustomer()
                {
                    OrderId = existingOrder.OrderId,
                    BaseProductId = existingReturnedGoodItem.BaseProductId,
                    BaseProductName = existingReturnedGoodItem.BaseProductName,
                    ProductVariantId = existingReturnedGoodItem.ProductVariantId,
                    ProductVariantColor = existingReturnedGoodItem.ProductColor,
                    ProductVariantSize = existingReturnedGoodItem.ProductSize,
                    PricePerItem = existingReturnedGoodItem.PricePaidPerItem,
                    Quantity = cancelRefundItem.Quantity
                };

                await _db.returnedItemsFromCustomers.AddAsync( returnItemFromCustomer );
                await _db.SaveChangesAsync();
            }
            //TODO remove quantity or item from items Good For Refund
            existingReturnedGoodItem.Quantity = existingReturnedGoodItem.Quantity
                - cancelRefundItem.Quantity;
            if(existingReturnedGoodItem.Quantity  ==  0)
            {
                _db.ItemsGoodForRefund.Remove(existingReturnedGoodItem);
            }

            await _db.SaveChangesAsync();
            //TODO deal with refund amounts
            existingReturnRequest.totalRequestForRefund =
                existingReturnRequest.totalRequestForRefund -
                (existingReturnedGoodItem.PricePaidPerItem * cancelRefundItem.Quantity);

            existingReturnRequest.totalAmountRefunded -=
                (existingReturnedGoodItem.PricePaidPerItem * cancelRefundItem.Quantity);

            await _db.SaveChangesAsync();

            return new Response_RefundIsSuccess()
            {
                isSuccess = false,
                Message = "Item for refund removed successfully"
            };
        }
        public async Task<Response_RefundIsSuccess> AddReturnedBadItem(Request_AddBadRefundItem addBadRefundItem)
        {
            //TODO Check if return request exist
            var existingReturnRequest = await _db.ItemReturnRequests
                .FirstOrDefaultAsync(ir=>ir.ExchangeUniqueIdentifier
                == addBadRefundItem.ExchangeUniqueIdentifier);
            if(existingReturnRequest == null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No Return Request exist with given Exchange Unique Id"
                };
            }
            //TODO Check if Product Variant exist
            var existingProductVariant = await _db.ProductVariants
                .FirstOrDefaultAsync(pv=>pv.Id  == addBadRefundItem.ProductVariantId);
            if(existingProductVariant == null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No product variant exist with given Product Variant Id"
                };
            }
            //TODO Check if item exist in Returned Items From Customer
            var existingReturnItem = await  _db.returnedItemsFromCustomers
                .FirstOrDefaultAsync(ri=>ri.ProductVariantId
                == addBadRefundItem.ProductVariantId);
            if(existingReturnItem == null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No Return Item Exist with given Product Variant Id"
                };
            }
            if(existingReturnItem.Quantity < addBadRefundItem.Quantity)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "Not enough return items at customer to move to bad items"
                };
            }
            //TODO check if item Bad For Refund exist and deal with quantity accordingly
            var existingRefundRequest = await _db.ItemReturnRequests
                .Include(ir=>ir.itemsBadForRefund)
                .FirstOrDefaultAsync(ir=>ir.ExchangeUniqueIdentifier
                == addBadRefundItem.ExchangeUniqueIdentifier);
            if(existingRefundRequest == null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No existing refund request found"
                };
            }

            bool itemExsitInBadForRefund = false;
            int itemBadForRefundId = 0;

            foreach(var item in existingRefundRequest.itemsBadForRefund)
            {
                if(item.ProductVariantId == addBadRefundItem.ProductVariantId)
                {
                    itemExsitInBadForRefund = true;
                    itemBadForRefundId = item.Id;
                }
            }

            if(itemExsitInBadForRefund == true)
            {
                var badItem = await _db.ItemsBadForRefund
                    .FirstOrDefaultAsync(ib => ib.Id == itemBadForRefundId);
                badItem.Quantity = badItem.Quantity + addBadRefundItem.Quantity;
                await _db.SaveChangesAsync();
            }
            else
            {
                var itemBadForRefund = new ItemBadForRefund()
                {
                    ItemReturnRequestId = existingReturnRequest.Id,
                    BaseProductId = existingReturnItem.BaseProductId,
                    BaseProductName = existingReturnItem.BaseProductName,
                    ProductVariantId = existingReturnItem.ProductVariantId,
                    ProductColor = existingReturnItem.ProductVariantColor,
                    ProductSize = existingReturnItem.ProductVariantSize,
                    PricePaidPerItem = existingReturnItem.PricePerItem,
                    Quantity = addBadRefundItem.Quantity,
                    ReasonForNotRefunding = addBadRefundItem.ReasonMessage
                };

                await _db.ItemsBadForRefund.AddAsync(itemBadForRefund);
                await _db.SaveChangesAsync();
            }
            //TODO Deal With Returned items quantity/removal
            existingReturnItem.Quantity = existingReturnItem.Quantity
                - addBadRefundItem.Quantity;

            var pricePerItemPaid = existingReturnItem.PricePerItem;

            if(existingReturnItem.Quantity ==  0)
            {
                _db.returnedItemsFromCustomers.Remove(existingReturnItem);
            }
            await  _db.SaveChangesAsync();
            //TODO Deal with amount of refund
            existingRefundRequest.totalRequestForRefund +=
                (pricePerItemPaid * addBadRefundItem.Quantity);

            existingRefundRequest.totalAmountNotRefunded +=
                (pricePerItemPaid * addBadRefundItem.Quantity);

            await _db.SaveChangesAsync();

            return new Response_RefundIsSuccess()
            {
                isSuccess = true,
                Message = "Returned Item Moved To Not Refundable Items"
            };

        }

        public async Task<Response_RefundIsSuccess> CancelBadReturnItem(Request_CancelRefundItem cancelRefundItem)
        {
            //TODO Get Bad Item by Id
            var existingBadItem = await _db.ItemsBadForRefund
                .FirstOrDefaultAsync(ib => ib.Id == cancelRefundItem.ReturnItemId);

            if(existingBadItem == null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No item at non refundable items found"
                };
            }
            //TODO Get Return Request
            var existingReturnRequest = await _db.ItemReturnRequests
                .FirstOrDefaultAsync(ir => ir.Id == existingBadItem.ItemReturnRequestId);
            if(existingReturnRequest == null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No Return Request Found"
                };
            }
            //TODO For Returned Item From Customer add quantity or new item
            var returnedItemFromCustomer = await _db.returnedItemsFromCustomers
                .FirstOrDefaultAsync(ri => ri.OrderId == existingReturnRequest.OrderId
                && ri.ProductVariantId == existingBadItem.ProductVariantId);
            if(returnedItemFromCustomer != null)
            {
                returnedItemFromCustomer.Quantity += cancelRefundItem.Quantity;
                await _db.SaveChangesAsync();
            }
            else
            {
                var newReturnedItemFromCustomer = new ReturnedItemsFromCustomer()
                {
                    OrderId = existingReturnRequest.OrderId,
                    BaseProductId = existingBadItem.BaseProductId,
                    BaseProductName = existingBadItem.BaseProductName,
                    ProductVariantId = existingBadItem.ProductVariantId,
                    ProductVariantColor = existingBadItem.ProductColor,
                    ProductVariantSize = existingBadItem.ProductSize,
                    PricePerItem = existingBadItem.PricePaidPerItem,
                    Quantity = cancelRefundItem.Quantity
                };
                await _db.returnedItemsFromCustomers.AddAsync(newReturnedItemFromCustomer);
                await _db.SaveChangesAsync();
            }

            var pricePaidPerItem = existingBadItem.PricePaidPerItem;

            //TODO remove quantity or item from Bad Return Items
            existingBadItem.Quantity -= cancelRefundItem.Quantity;
            if(existingBadItem.Quantity == 0)
            {
                _db.ItemsBadForRefund.Remove(existingBadItem);
            }

            await _db.SaveChangesAsync();
            //TODO Deal with total amounts
            existingReturnRequest.totalRequestForRefund =
                existingReturnRequest.totalRequestForRefund -
                (pricePaidPerItem * cancelRefundItem.Quantity);

            existingReturnRequest.totalAmountNotRefunded =
                existingReturnRequest.totalAmountNotRefunded
                - (pricePaidPerItem * cancelRefundItem.Quantity);

            await _db.SaveChangesAsync();

            return new Response_RefundIsSuccess()
            {
                isSuccess = true,
                Message = "Bad Refund Item Moved To Returned Items"
            };


        }

        public async Task<Response_RefundIsSuccess> SetOrderAsRefunded(string exchangeUniqueIdentifier)
        {
            var existingReturnRequest = await _db.ItemReturnRequests
                .FirstOrDefaultAsync(ir => ir.ExchangeUniqueIdentifier
                == exchangeUniqueIdentifier);
            if(existingReturnRequest == null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No Return Request Found With Given Exchange Unique Id"
                };
            }

            existingReturnRequest.RequestRefunded = true;
            await _db.SaveChangesAsync();

            return new Response_RefundIsSuccess()
            {
                isSuccess = true,
                Message = "Return request set as refunded"
            };
        }

        public async Task<Response_RefundIsSuccess> CancelOrderAsRefunded(string exchangeUniqueIdentifier)
        {
            var existingReturnRequest =await _db.ItemReturnRequests
                .FirstOrDefaultAsync(ir=>ir.ExchangeUniqueIdentifier 
                == exchangeUniqueIdentifier);

            if(existingReturnRequest  ==  null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No return request found with given exchange unique Id"
                };
            }

            existingReturnRequest.RequestRefunded = false;
            await _db.SaveChangesAsync();

            return new Response_RefundIsSuccess()
            {
                isSuccess = true,
                Message = "Return request set as not refunded"
            };
        }

        public async Task<Response_RefundIsSuccess> SetOrderAsDone(string exchangeUniqueIdentifier)
        {
            var existingReturnRequest = await _db.ItemReturnRequests
                .FirstOrDefaultAsync(ir => ir.ExchangeUniqueIdentifier
                == exchangeUniqueIdentifier);
            if(existingReturnRequest == null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No return request found with given Exchange Unique Id"
                };
            }
            if(existingReturnRequest.RequestRefunded == false)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "Request is not refunded, refund request first"
                };
            }
            existingReturnRequest.RequestClosed = true;
            await _db.SaveChangesAsync();

            return new Response_RefundIsSuccess()
            {
                isSuccess = true,
                Message = "Refund request closed"
            };

        }

        public async Task<Response_RefundIsSuccess> CancelOrderAsDone(string exchangeUniqueIdentifier)
        {
            var existingReturnRequest = await _db.ItemReturnRequests
                .FirstOrDefaultAsync(ir => ir.ExchangeUniqueIdentifier
                == exchangeUniqueIdentifier);
            if(existingReturnRequest == null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No refund request found with given Exchange Unique Id"
                };
            }
            existingReturnRequest.RequestClosed = false;
            await _db.SaveChangesAsync();

            return new Response_RefundIsSuccess()
            {
                isSuccess = true,
                Message = "Return Request set as not closed"
            };
        }

        public async Task<Response_GoodRefundItems> GetAllGoodRefundItems(string exchangeUniqueIdentifier)
        {
            var existingReturnRequest  = await _db.ItemReturnRequests
                .Include(ir=>ir.itemsGoodForRefund)
                .FirstOrDefaultAsync(ir=>ir.ExchangeUniqueIdentifier
                == exchangeUniqueIdentifier);

            if(existingReturnRequest == null)
            {
                return new Response_GoodRefundItems()
                {
                    isSuccess = false,
                    Message = "No return request found with given Exchange Unique Id"
                };
            }

            return new Response_GoodRefundItems()
            {
                isSuccess = true,
                Message = "Return items in good condition",
                OrderUniqueIdentifier = existingReturnRequest.OrderUniqueIdentifier,
                ExchangeUniqueIdentifier = existingReturnRequest.ExchangeUniqueIdentifier,
                itemsGoodForRefund = existingReturnRequest.itemsGoodForRefund.ToList()
            };
        }

        public async Task<Response_BadRefundItems> GetAllBadRefundItems(string exchangeUniqueIdentifier)
        {
            var existingReturnRequest = await _db.ItemReturnRequests
                .Include(ir => ir.itemsBadForRefund)
                .FirstOrDefaultAsync(ir => ir.ExchangeUniqueIdentifier
                == exchangeUniqueIdentifier);

            if(existingReturnRequest == null)
            {
                return new Response_BadRefundItems()
                {
                    isSuccess = false,
                    Message = "No return request found with given exchange unique Id"
                };
            }

            return new Response_BadRefundItems()
            {
                isSuccess = true,
                Message = "Returned items in bad condition",
                OrderUniqueIdentifier = existingReturnRequest.OrderUniqueIdentifier,
                ExchangeUniqueIdentifier = existingReturnRequest.ExchangeUniqueIdentifier,
                ItemsBadForRefund = existingReturnRequest.itemsBadForRefund.ToList()
            };
        }


        public async Task<Response_RefundIsSuccess> AllItemsCheckedSendEmail(string exchangeUniqueIdentifier)
        {
            var existingReturnOrder = await _db.ItemReturnRequests
                .Include(ir => ir.itemsGoodForRefund)
                .Include(ir => ir.itemsBadForRefund)
                .FirstOrDefaultAsync(ir => ir.ExchangeUniqueIdentifier
                == exchangeUniqueIdentifier);

            if(existingReturnOrder == null)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "No return request found with given exchange unique Id"
                };
            }

            if(existingReturnOrder.RequestClosed == false)
            {
                return new Response_RefundIsSuccess()
                {
                    isSuccess = false,
                    Message = "Close request first, before sending confirmation email"
                };
            }

            string fromEmail = _configuration.GetSection("SendGridEmailSettings")
                .GetValue<string>("FromEmail");
            string fromName = _configuration.GetSection("SendGridEmailSettings")
                .GetValue<string>("FromName");

            var msg = new SendGridMessage()
            {
                From = new EmailAddress(fromEmail, fromName),
                Subject = "Item Return Details",
                HtmlContent = EmailTemplates.RefundTemplate(existingReturnOrder)
            };

            var email = existingReturnOrder.UserEmail; //Add to next line, leave for testing

            msg.AddTo("vaceintech@gmail.com");
            var response = await _sendGridClient.SendEmailAsync(msg);

            string message = response.IsSuccessStatusCode ? "Email sent successfully"
                : "Email failed to send";
            bool messageSuccess = response.IsSuccessStatusCode;

            return new Response_RefundIsSuccess()
            {
                isSuccess = messageSuccess,
                Message = message
            };

        }
    }
}
