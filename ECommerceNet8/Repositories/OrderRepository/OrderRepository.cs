using ECommerceNet8.Data;
using ECommerceNet8.DTOs.OrderDtos.Request;
using ECommerceNet8.DTOs.OrderDtos.Response;
using ECommerceNet8.Models.AuthModels;
using ECommerceNet8.Models.OrderModels;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SendGrid;

namespace ECommerceNet8.Repositories.OrderRepository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApiUser> _user;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly ISendGridClient _sendGridClient;

        public OrderRepository(ApplicationDbContext db,
            UserManager<ApiUser> user,
            IWebHostEnvironment webHostEnvironment,
            IConfiguration configuration,
            ISendGridClient sendGridClient)
        {
            _db = db;
            _user = user;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _sendGridClient = sendGridClient;
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            //TODO ADD OTHER INCULDES LATER
            var orders = await _db.Orders
                .Include(o => o.OriginalOrderFromCustomer)
                .ThenInclude(oo => oo.OrderItems)
                .Include(o => o.OriginalOrderFromCustomer)
                .ThenInclude(oo => oo.pdfInfo)
                .Include(o => o.ItemsAtCustomer)

                .Include(o=>o.ItemExchangeRequests)
                .ThenInclude(ie=>ie.exchangeOrderItems)
                .Include(o=>o.ItemExchangeRequests)
                .ThenInclude(ie=>ie.exchangeItemsCanceled)
                .Include(o=>o.ItemExchangeRequests)
                .ThenInclude(ie => ie.exchangeItemsPending)
                .Include(o=>o.ItemExchangeRequests)
                .ThenInclude(ie=>ie.exchangeConfirmedPdfInfo)

                .Include(o=>o.itemReturnRequests)
                .ThenInclude(ir=>ir.itemsGoodForRefund)
                .Include(o=>o.itemReturnRequests)
                .ThenInclude(ir=>ir.itemsBadForRefund)

                .ToListAsync();

            return orders;
        }
        public async Task<Order> GetOrder(string OrderUniqueIdentifier)
        {
            //TODO ADD OTHER INCLUDES LATER
            var order = await _db.Orders
                .Include(o => o.OriginalOrderFromCustomer)
                .ThenInclude(oo => oo.OrderItems)
                .Include(o => o.OriginalOrderFromCustomer)
                .ThenInclude(oo => oo.pdfInfo)
                .Include(o => o.ItemsAtCustomer)

                .Include(o => o.ItemExchangeRequests)
                .ThenInclude(ie => ie.exchangeOrderItems)
                .Include(o => o.ItemExchangeRequests)
                .ThenInclude(ie => ie.exchangeItemsCanceled)
                .Include(o => o.ItemExchangeRequests)
                .ThenInclude(ie => ie.exchangeItemsPending)
                .Include(o => o.ItemExchangeRequests)
                .ThenInclude(ie => ie.exchangeConfirmedPdfInfo)

                .Include(o => o.itemReturnRequests)
                .ThenInclude(ir => ir.itemsGoodForRefund)
                .Include(o => o.itemReturnRequests)
                .ThenInclude(ir => ir.itemsBadForRefund)

                .FirstOrDefaultAsync(o=>o.OrderUniqueIdentifier == OrderUniqueIdentifier);

            return order;
        }

        public async Task<Order> GetOrderForPdf(string OrderUniqueIdentifier)
        {
            var order = await _db.Orders
                .Include(o=>o.OriginalOrderFromCustomer)
                .ThenInclude(oo=> oo.pdfInfo)
                .FirstOrDefaultAsync(o=>o.OrderUniqueIdentifier==OrderUniqueIdentifier);

            return order;
        }

        public async Task<IEnumerable<Order>> GetAllOrderByDate(Request_OrderDate orderDate)
        {
            DateTime startDate =
                new DateTime(orderDate.StartYear, orderDate.StartMonth, orderDate.StartDay);
            DateTime endDate =
                new DateTime(orderDate.EndYear, orderDate.EndMonth, orderDate.EndDay);

            //TODO ADD OTHER INCLUDES LATER

            var orders = await _db.Orders
                .Where(o => o.OrderTime >= startDate && o.OrderTime < endDate)
                .Include(o => o.OriginalOrderFromCustomer)
                .ThenInclude(oo => oo.OrderItems)
                .Include(o => o.OriginalOrderFromCustomer)
                .ThenInclude(oo => oo.pdfInfo)
                .Include(o => o.ItemsAtCustomer)

                .Include(o => o.ItemExchangeRequests)
                .ThenInclude(ie => ie.exchangeOrderItems)
                .Include(o => o.ItemExchangeRequests)
                .ThenInclude(ie => ie.exchangeItemsCanceled)
                .Include(o => o.ItemExchangeRequests)
                .ThenInclude(ie => ie.exchangeItemsPending)
                .Include(o => o.ItemExchangeRequests)
                .ThenInclude(ie => ie.exchangeConfirmedPdfInfo)

                .ToArrayAsync();

            return orders;

        }

        public async Task<IEnumerable<Order>> GetNotSentOrders()
        {
            var orders = await _db.Orders
                .Where(o=>o.OriginalOrderFromCustomer.ItemSent == false)
                .Include(o => o.OriginalOrderFromCustomer)
                .ThenInclude(oo => oo.OrderItems)
                .Include(o => o.OriginalOrderFromCustomer)
                .ThenInclude(oo => oo.pdfInfo)
                .Include(o => o.ItemsAtCustomer)
                .ToListAsync();

            return orders;
        }

        public async Task<Response_Order> GenerateOrder(string userId, int userAddressId, int shippingTypeId)
        {
            Order order = new Order();
            order.OrderUniqueIdentifier = await GenerateId();

            decimal TotalPrice = 0;
            decimal TotalDiscount = 0;

            var user = await _user.FindByIdAsync(userId);
            order.UserId = userId;
            order.UserFirstName = user.FirstName;
            order.UserLastName = user.LastName;
            order.UserEmail = user.Email;

            var address = await _db.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == userAddressId);

            if (address.AppartmentNumber == null)
            {
                order.AppartmentNumber = null;
            }
            else
            {
                order.AppartmentNumber = address.AppartmentNumber;
            }

            order.HouseNumber = address.HouseNumber;
            order.Street = address.Street;
            order.City = address.City;
            order.Region = address.Region;
            order.PostalCode = address.PostalCode;
            order.Country = address.Country;

            order.OrderTime = DateTime.UtcNow;

            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();

            var shoppingCart = await _db.ShoppingCarts
                .Include(sc=>sc.CartItems)
                .FirstOrDefaultAsync(sc=>sc.ApiUserId  == userId);

            if(shoppingCart == null)
            {
                var existingOrderToRemove = await _db.Orders
                    .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
                _db.Orders.Remove(existingOrderToRemove);
                await _db.SaveChangesAsync();

                return new Response_Order()
                {
                    isSuccess = false,
                    Message = "No Existing Shopping Cart For User",
                    OrderUniqueIdentifier = "0"
                };
            }


            var hasEnoughItems = true;

            foreach(var item in shoppingCart.CartItems)
            {
                bool hasEnough = await HasEnoughItems(item.ProductVariantId,
                    item.Quantity);
                if(hasEnough == false)
                {
                    hasEnoughItems = false;
                }
            }

            if(hasEnoughItems == false)
            {
                var existingOrderToRemove = await _db.Orders
                    .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
                _db.Orders.Remove(existingOrderToRemove);
                await _db.SaveChangesAsync();

                return new Response_Order()
                {
                    isSuccess = false,
                    Message = "Some Items Dont Have Enough Quantity In Stock",
                    OrderUniqueIdentifier = "0"
                };
            }

            foreach(var item in shoppingCart.CartItems)
            {
                await RemoveItemQuantity(item.ProductVariantId, item.Quantity);
            }

            OrderFromCustomer orderFromCustomer = new OrderFromCustomer();
            orderFromCustomer.OrderId = order.OrderId;
            orderFromCustomer.RefundMade = false;
            orderFromCustomer.OrderCanceled = false;
            orderFromCustomer.ItemSent = false;

            await _db.OrdersFromCustomers.AddAsync(orderFromCustomer);
            await _db.SaveChangesAsync();
            order.OrderFromCustomerId = orderFromCustomer.Id;
            await _db.SaveChangesAsync();

            foreach(var item in shoppingCart.CartItems)
            {
                OrderItem orderItem = new OrderItem();
                var productVariant = await _db.ProductVariants
                    .Include(pv => pv.productColor)
                    .Include(pv => pv.productSize)
                    .FirstOrDefaultAsync(pv => pv.Id == item.ProductVariantId);
                var baseProduct = await _db.BaseProducts
                    .FirstOrDefaultAsync(bp => bp.Id == productVariant.BaseProductId);

                orderItem.OrderFromCustomerId = orderFromCustomer.Id;
                orderItem.BaseProductId = baseProduct.Id;
                orderItem.BaseProductName = baseProduct.Name;
                orderItem.ProductVariantId = productVariant.Id;
                orderItem.ProductVariantColor = productVariant.productColor.Name;
                orderItem.ProductVariantSize = productVariant.productSize.Name;
                orderItem.Quantity = item.Quantity;
                orderItem.Price = baseProduct.Price;
                orderItem.Discount = baseProduct.Discount;
                orderItem.PricePerItem = baseProduct.TotalPrice;

                var totalPrice = baseProduct.TotalPrice * item.Quantity;
                var totalPriceDecimal = decimal.Round(totalPrice, 2);

                orderItem.TotalPrice = totalPriceDecimal;

                await _db.OrdersItems.AddAsync(orderItem);
                await _db.SaveChangesAsync();

                TotalPrice += totalPriceDecimal;

                var discount = (baseProduct.Price * baseProduct.Discount / 100);
                var totalDiscountDecimal = decimal.Round(discount, 2);
                TotalDiscount += totalDiscountDecimal;

            }

            var shippingType = await _db.ShippingTypes
                .FirstOrDefaultAsync(st => st.ShippingTypeId == shippingTypeId);

            orderFromCustomer.ShippingTypeId = shippingType.ShippingTypeId;
            orderFromCustomer.ShippingFirmName = shippingType.ShippingFirmName;
            orderFromCustomer.ShippingPrice = shippingType.Price;

            orderFromCustomer.Price = TotalPrice;
            var totalToPay = TotalPrice + shippingType.Price;
            orderFromCustomer.TotalPrice = totalToPay;
            orderFromCustomer.Discount = TotalDiscount;

            await _db.SaveChangesAsync();

            var existingOrder = await _db.Orders
                .Include(o => o.OriginalOrderFromCustomer)
                .ThenInclude(oo => oo.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderFromCustomer.OrderId);

            //GERERATE PDF
            PdfInfo pdfInfo = new PdfInfo();
            pdfInfo.OrderFromCustomerId = orderFromCustomer.Id;
            pdfInfo.Name = "PDF info for " + order.OrderUniqueIdentifier + " order";
            pdfInfo.Added = DateTime.UtcNow;
            pdfInfo.Path = await CreatePdf(existingOrder);

            await _db.PdfInfos.AddAsync(pdfInfo);
            await _db.SaveChangesAsync();


            orderFromCustomer.PdfInfoId = pdfInfo.Id;
            await _db.SaveChangesAsync();

            return new Response_Order()
            {
                isSuccess = true,
                Message = "Order Created Successfully",
                OrderUniqueIdentifier = order.OrderUniqueIdentifier
            };
        }

        public async Task<Response_ItemsAtCustomer> GetItemsAtCustomer(string OrderUniqueIdentifier)
        {
            var order = await _db.Orders
                .Include(o=>o.ItemsAtCustomer)
                .FirstOrDefaultAsync(o=>o.OrderUniqueIdentifier == OrderUniqueIdentifier);

            if(order == null)
            {
                return new Response_ItemsAtCustomer()
                {
                    isSuccess = false,
                    Message = "No Order Found With Given Id"
                };
            }

            return new Response_ItemsAtCustomer()
            { 
                isSuccess = true,
                Message = "Items Retrieved",
                OrderId = order.OrderId,
                OrderUniqueIdentifier = order.OrderUniqueIdentifier,
                ItemsAtCustomer = order.ItemsAtCustomer
            
            };

        }
        public async Task<Response_Order> MarkOrderAsSent(int orderId)
        {
            var existingOrder = await _db.Orders
                .Include(o => o.OriginalOrderFromCustomer)
                .ThenInclude(oo => oo.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if(existingOrder == null)
            {
                return new Response_Order
                {
                    isSuccess = false,
                    Message = "No Order Found With Given Id",
                    OrderUniqueIdentifier = "0"
                };
            }

            if(existingOrder.OriginalOrderFromCustomer.ItemSent == true)
            {
                return new Response_Order()
                {
                    isSuccess = false,
                    Message = "Order Already Mark As Sent",
                    OrderUniqueIdentifier = existingOrder.OrderUniqueIdentifier
                };
            }

            existingOrder.OriginalOrderFromCustomer.ItemSent = true;
            await _db.SaveChangesAsync();

            foreach(var orderItem in existingOrder.OriginalOrderFromCustomer.OrderItems)
            {
                ItemAtCustomer itemAtCustomer = new ItemAtCustomer();

                itemAtCustomer.OrderId = orderId;
                itemAtCustomer.BaseProductId = orderItem.BaseProductId;
                itemAtCustomer.BaseProductName = orderItem.BaseProductName;
                itemAtCustomer.ProductVariantId = orderItem.ProductVariantId;
                itemAtCustomer.ProductVariantColor = orderItem.ProductVariantColor;
                itemAtCustomer.ProductVariantSize = orderItem.ProductVariantSize;
                itemAtCustomer.PricePaidPerItem = orderItem.PricePerItem;
                itemAtCustomer.Quantity = orderItem.Quantity;

                await _db.ItemAtCustomers.AddAsync(itemAtCustomer);
                await _db.SaveChangesAsync();
            }

            return new Response_Order
            {
                isSuccess = true,
                Message = "Order Marked AS Sent",
                OrderUniqueIdentifier = existingOrder.OrderUniqueIdentifier
            };

        }
        public async Task<Response_Order> MarkOrderAsNotSent(int orderId)
        {
            var existingOrder = await _db.Orders
                .Include(o => o.OriginalOrderFromCustomer)
                .Include(o => o.ItemsAtCustomer)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if(existingOrder == null)
            {
                return new Response_Order
                {
                    isSuccess = false,
                    Message = "No Order Found With Given Id",
                    OrderUniqueIdentifier = "0"
                };
            }

            if(existingOrder.OriginalOrderFromCustomer.RefundMade == true)
            {
                return new Response_Order()
                {
                    isSuccess = false,
                    Message = "Order Already Refunded",
                    OrderUniqueIdentifier = existingOrder.OrderUniqueIdentifier
                };
            }

            _db.ItemAtCustomers.RemoveRange(existingOrder.ItemsAtCustomer);
            existingOrder.OriginalOrderFromCustomer.ItemSent = false;

            await _db.SaveChangesAsync();

            return new Response_Order()
            {
                isSuccess = true,
                Message = "Order Marked As Not Sent",
                OrderUniqueIdentifier = existingOrder.OrderUniqueIdentifier
            };
        }




        #region  HelperFunctions

        private async Task<string> GenerateId()
        {
            char[] Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            Random random = new Random();

            string randomLetters = "";
            for(int i = 0; i<3; i++)
            {
                randomLetters += Letters[random.Next(Letters.Length)];
            }
            int randomNumber = random.Next(100000000, 999999999);

            string UniqueIdentifier = randomLetters +  randomNumber.ToString();

            var existingIdentifier  = await _db.Orders
                .FirstOrDefaultAsync(o=>o.OrderUniqueIdentifier == UniqueIdentifier);
            if(existingIdentifier != null)
            {
                GenerateId();
            }

            return UniqueIdentifier;
        }

        private async Task<bool> HasEnoughItems(int productVariantId, int quantity)
        {
            var existingProductVariant = await _db.ProductVariants
                .FirstOrDefaultAsync(pv=>pv.Id == productVariantId);
            if(existingProductVariant.Quantity < quantity)
            {
                return false;
            }
            return true;
        }

        private async Task<bool> RemoveItemQuantity(int productVariantId, int quantity)
        {
            var existingProductVariant = await _db.ProductVariants
                .FirstOrDefaultAsync(pv=>pv.Id == productVariantId);
            existingProductVariant.Quantity = existingProductVariant.Quantity - quantity;
            await _db.SaveChangesAsync();

            return true;
        }

        #endregion

        #region PDF FUNCTIONS
        public async Task<string> CreatePdf(Order existingOrder)
        {

            var date = DateTime.Now.ToShortDateString().ToString();
            var dateNormilized = date.Replace("/", "_");
            string fileName = "PDF_" + dateNormilized + "_" + DateTime.UtcNow.Millisecond + ".pdf";

            string folderPath = _webHostEnvironment.WebRootPath + $"\\PDF\\{existingOrder.UserId}\\{existingOrder.OrderUniqueIdentifier}";
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

            Paragraph name = new Paragraph(existingOrder.UserFirstName + " " + existingOrder.UserLastName)
                .SetFont(font)
                .SetFontSize(12)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
            doc.Add(name);

            Paragraph email = new Paragraph(existingOrder.UserEmail)
                .SetFont(font)
                .SetFontSize(14)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
            doc.Add(email);

            Paragraph street = new Paragraph(existingOrder.Street)
                .SetFont(font)
                .SetFontSize(14)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
            doc.Add(street);

            if (existingOrder.AppartmentNumber == null || existingOrder.AppartmentNumber == 0)
            {
                Paragraph addressHouseNum = new Paragraph(existingOrder.HouseNumber.ToString())
                .SetFont(font)
                .SetFontSize(14)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
                doc.Add(addressHouseNum);
            }
            else
            {
                Paragraph addressHouseApp = new Paragraph(existingOrder.HouseNumber.ToString() + "-" + existingOrder.AppartmentNumber.ToString())
                .SetFont(font)
                .SetFontSize(14)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
                doc.Add(addressHouseApp);
            }

            Paragraph addressCity = new Paragraph(existingOrder.City)
                .SetFont(font)
                .SetFontSize(14)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
            doc.Add(addressCity);

            Paragraph addressRegion = new Paragraph(existingOrder.Region)
                .SetFont(font)
                .SetFontSize(14)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
            doc.Add(addressRegion);

            Paragraph addressCountry = new Paragraph(existingOrder.Country)
                .SetFont(font)
                .SetFontSize(14)
                .SetTextAlignment(aligmentLeft)
                .SetMarginBottom(0);
            doc.Add(addressCountry);

            Paragraph postalCode = new Paragraph(existingOrder.PostalCode)
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

            Paragraph orderId = new Paragraph("Invoice for order: " + existingOrder.OrderUniqueIdentifier)
               .SetFont(font)
               .SetFontSize(12)
               .SetTextAlignment(aligmentCenter)
               .SetPaddingTop(5);
            doc.Add(orderId);


            //order items table

            Table ItemTable = new Table(7);
            ItemTable.SetMarginTop(20);
            ItemTable.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            ItemTable.SetWidth(500);


            //iText.Kernel.Colors
            Color textColorTableHeadings = new DeviceRgb(255, 255, 255);
            Color bgColorTableHeadings = new DeviceRgb(1, 1, 1);

            Cell cell1 = new Cell().Add(new Paragraph("PRODUCT NAME")
                .SetFontColor(textColorTableHeadings)
                .SetFontSize(8)
                .SetTextAlignment(aligmentCenter));
            cell1.SetBackgroundColor(bgColorTableHeadings);
            cell1.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
            ItemTable.AddCell(cell1);

            Cell cell2 = new Cell().Add(new Paragraph("PRODUCT COLOR")
                .SetFontColor(textColorTableHeadings)
                .SetFontSize(8)
                .SetTextAlignment(aligmentCenter));
            cell2.SetBackgroundColor(bgColorTableHeadings);
            cell2.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
            ItemTable.AddCell(cell2);

            Cell cell3 = new Cell().Add(new Paragraph("PRODUCT SIZE")
                .SetFontColor(textColorTableHeadings)
                .SetFontSize(8)
                .SetTextAlignment(aligmentCenter));
            cell3.SetBackgroundColor(bgColorTableHeadings);
            cell3.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
            ItemTable.AddCell(cell3);

            Cell cell4 = new Cell().Add(new Paragraph("QUANTITY")
               .SetFontColor(textColorTableHeadings)
               .SetFontSize(8)
               .SetTextAlignment(aligmentCenter));
            cell4.SetBackgroundColor(bgColorTableHeadings);
            cell4.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
            ItemTable.AddCell(cell4);

            Cell cell5 = new Cell().Add(new Paragraph("PRICE")
               .SetFontColor(textColorTableHeadings)
               .SetFontSize(8)
               .SetTextAlignment(aligmentCenter));
            cell5.SetBackgroundColor(bgColorTableHeadings);
            cell5.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
            ItemTable.AddCell(cell5);

            Cell cell6 = new Cell().Add(new Paragraph("DISCOUNT IN %")
               .SetFontColor(textColorTableHeadings)
               .SetFontSize(8)
               .SetTextAlignment(aligmentCenter));
            cell6.SetBackgroundColor(bgColorTableHeadings);
            cell6.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
            ItemTable.AddCell(cell6);

            Cell cell7 = new Cell().Add(new Paragraph("TOTAL PRICE")
                .SetFontColor(textColorTableHeadings)
                .SetFontSize(8)
                .SetTextAlignment(aligmentCenter));
            cell7.SetBackgroundColor(bgColorTableHeadings);
            cell7.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
            ItemTable.AddCell(cell7);


            foreach (var item in existingOrder.OriginalOrderFromCustomer.OrderItems)
            {

                Cell CellProductName = new Cell().Add(new Paragraph(item.BaseProductName)
                    .SetTextAlignment(aligmentCenter)
                    .SetFontSize(8));
                CellProductName.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ItemTable.AddCell(CellProductName);

                Cell CellProductColor = new Cell().Add(new Paragraph(item.ProductVariantColor)
                    .SetTextAlignment(aligmentCenter)
                    .SetFontSize(8));
                CellProductColor.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ItemTable.AddCell(CellProductColor);

                Cell CellProductSize = new Cell().Add(new Paragraph(item.ProductVariantSize)
                   .SetTextAlignment(aligmentCenter)
                   .SetFontSize(8));
                CellProductSize.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ItemTable.AddCell(CellProductSize);

                Cell CellProductQuantity = new Cell().Add(new Paragraph(item.Quantity.ToString())
                   .SetTextAlignment(aligmentCenter)
                   .SetFontSize(8));
                CellProductQuantity.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ItemTable.AddCell(CellProductQuantity);

                Cell CellProductPrice = new Cell().Add(new Paragraph(item.Price.ToString())
                   .SetTextAlignment(aligmentCenter)
                   .SetFontSize(8));
                CellProductPrice.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ItemTable.AddCell(CellProductPrice);

                Cell CellProductDiscount = new Cell().Add(new Paragraph(item.Discount.ToString())
                   .SetTextAlignment(aligmentCenter)
                   .SetFontSize(8));
                CellProductDiscount.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ItemTable.AddCell(CellProductDiscount);


                Cell CellProdutTotalPrice = new Cell().Add(new Paragraph(item.TotalPrice.ToString())
                   .SetTextAlignment(aligmentCenter)
                   .SetFontSize(8));
                CellProductDiscount.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
                ItemTable.AddCell(CellProdutTotalPrice);


                var totalDiscount = ((item.Price * item.Quantity) * item.Discount / 100);
                var totalDiscountDecimal = decimal.Round(totalDiscount, 2);
                TotalDiscountOfAllItems += totalDiscountDecimal;
                TotalPriceOfAllItems += item.TotalPrice;

            }

            doc.Add(ItemTable);



            Table PriceTable = new Table(2);
            PriceTable.SetMarginTop(20);
            PriceTable.SetHorizontalAlignment(HorizontalAlignment.CENTER);
            PriceTable.SetWidth(200);

            Cell cellDiscountName = new Cell().Add(new Paragraph("DISCOUNT")
                .SetFontColor(textColorTableHeadings)
                .SetFontSize(8)
                .SetTextAlignment(aligmentCenter));
            cellDiscountName.SetBackgroundColor(bgColorTableHeadings);
            cellDiscountName.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
            PriceTable.AddCell(cellDiscountName);

            Cell CellDiscountValue = new Cell().Add(new Paragraph("$" + TotalDiscountOfAllItems.ToString())
                   .SetTextAlignment(aligmentCenter)
                   .SetFontSize(8));
            CellDiscountValue.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
            PriceTable.AddCell(CellDiscountValue);

            Cell cellPriceName = new Cell().Add(new Paragraph("PRICE")
               .SetFontColor(textColorTableHeadings)
               .SetFontSize(8)
               .SetTextAlignment(aligmentCenter));
            cellPriceName.SetBackgroundColor(bgColorTableHeadings);
            cellPriceName.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
            PriceTable.AddCell(cellPriceName);

            Cell CellPriceValue = new Cell().Add(new Paragraph("$" + TotalPriceOfAllItems.ToString())
                   .SetTextAlignment(aligmentCenter)
                   .SetFontSize(8));
            CellDiscountValue.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
            PriceTable.AddCell(CellPriceValue);

            Cell cellShippingPriceName = new Cell().Add(new Paragraph("SHIPPING PRICE")
               .SetFontColor(textColorTableHeadings)
               .SetFontSize(8)
               .SetTextAlignment(aligmentCenter));
            cellShippingPriceName.SetBackgroundColor(bgColorTableHeadings);
            cellShippingPriceName.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
            PriceTable.AddCell(cellShippingPriceName);

            Cell CellShippingPriceValue = new Cell().Add(new Paragraph("$" + existingOrder.OriginalOrderFromCustomer.ShippingPrice)
                   .SetTextAlignment(aligmentCenter)
                   .SetFontSize(8));
            CellDiscountValue.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
            PriceTable.AddCell(CellShippingPriceValue);

            var totalPriceInCurrency = TotalPriceOfAllItems + existingOrder.OriginalOrderFromCustomer.ShippingPrice;

            Cell cellTotalPriceName = new Cell().Add(new Paragraph("TOTAL PRICE")
              .SetFontColor(textColorTableHeadings)
              .SetFontSize(8)
              .SetTextAlignment(aligmentCenter));
            cellTotalPriceName.SetBackgroundColor(bgColorTableHeadings);
            cellTotalPriceName.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
            PriceTable.AddCell(cellTotalPriceName);

            Cell CellTotalPriceValue = new Cell().Add(new Paragraph("$" + totalPriceInCurrency)
                   .SetTextAlignment(aligmentCenter)
                   .SetFontSize(8));
            CellTotalPriceValue.SetBorder(new SolidBorder(ColorConstants.GRAY, 2));
            PriceTable.AddCell(CellTotalPriceValue);


            doc.Add(PriceTable);

            doc.Close();

            return path;
        }
        #endregion
    }
}
