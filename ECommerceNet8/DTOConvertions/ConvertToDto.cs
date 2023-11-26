using ECommerceNet8.DTOs.BaseProductDtos.CustomModels;
using ECommerceNet8.DTOs.ProductVariantDtos.CustomModels;
using ECommerceNet8.DTOs.RequestExchangeDtos.Models;
using ECommerceNet8.DTOs.RequestExchangeDtos.Response;
using ECommerceNet8.DTOs.ShoppingCartDtos.Models;
using ECommerceNet8.Models.OrderModels;
using ECommerceNet8.Models.ProductModels;
using ECommerceNet8.Models.ShoppingCartModels;

namespace ECommerceNet8.DTOConvertions
{
    public static class ConvertToDto
    {
        public static IEnumerable<Model_BaseProductCustom> ConvertToDtoListCustomProduct
            (this IEnumerable<BaseProduct> baseProducts)
        {
            var BaseProductCustomReturn = new List<Model_BaseProductCustom>();

            foreach(var baseProduct in baseProducts)
            {
                List<Model_BaseImageCustom> images= new List<Model_BaseImageCustom>();

                foreach(var imageBase  in baseProduct.imageBases)
                {
                    var baseImageCustom = new Model_BaseImageCustom()
                    {
                        Id = imageBase.Id,
                        BaseProductId = imageBase.BaseProductId,
                        AddedOn = imageBase.AddedOn,
                        staticPath = imageBase.StaticPath
                    };

                    images.Add(baseImageCustom);
                }

                List<Model_ProductVariantCustom> productVariants = 
                    new List<Model_ProductVariantCustom>();

                foreach(var productVariant in baseProduct.productVariants)
                {
                    var productColor = new Model_ProductColorCustom()
                    {
                        Id = productVariant.productColor.Id,
                        Name = productVariant.productColor.Name
                    };

                    var productSize = new Model_ProductSizeCustom()
                    {
                        Id = productVariant.productSize.Id,
                        Name = productVariant.productSize.Name
                    };

                    var productVariantCustom = new Model_ProductVariantCustom()
                    {
                        Id = productVariant.Id,
                        BaseProductId = productVariant.BaseProductId,
                        productColor = productColor,
                        productSize = productSize,
                        Quantity = productVariant.Quantity
                    };

                    productVariants.Add(productVariantCustom);
                }

                var baseProductCustom = new Model_BaseProductCustom()
                {
                    Id = baseProduct.Id,
                    Name = baseProduct.Name,
                    Description = baseProduct.Description,
                    mainCategory = baseProduct.MainCategory,
                    material = baseProduct.Material,
                    productVariants = productVariants,
                    ImagesBases = images,
                    Price = baseProduct.Price,
                    Discount = baseProduct.Discount,
                    TotalPrice = baseProduct.TotalPrice,
                };

                BaseProductCustomReturn.Add(baseProductCustom);
            }

            return BaseProductCustomReturn;
        }

        public static Model_BaseProductCustom ConvertToDtoCustomProduct(
            this BaseProduct baseProduct)
        {
            var BaseProductCustom = new Model_BaseProductCustom();

            List<Model_BaseImageCustom> images = new List<Model_BaseImageCustom>();

            foreach(var imageBase in baseProduct.imageBases)
            {
                var imageBaseCustom = new Model_BaseImageCustom()
                {
                    Id = imageBase.Id,
                    BaseProductId = imageBase.BaseProductId,
                    AddedOn = imageBase.AddedOn,
                    staticPath = imageBase.StaticPath
                };

                images.Add(imageBaseCustom);
            }

            List<Model_ProductVariantCustom> productVariants 
                = new List<Model_ProductVariantCustom>();

            foreach(var productVariant in baseProduct.productVariants)
            {
                var productColor = new Model_ProductColorCustom()
                {
                    Id = productVariant.productColor.Id,
                    Name = productVariant.productColor.Name
                };

                var productSize = new Model_ProductSizeCustom()
                {
                    Id = productVariant.productSize.Id,
                    Name = productVariant.productSize.Name
                };

                var productVariantCustom = new Model_ProductVariantCustom()
                {
                    Id = productVariant.Id,
                    BaseProductId = productVariant.BaseProductId,
                    productColor = productColor,
                    productSize = productSize,
                    Quantity = productVariant.Quantity
                };

                productVariants.Add(productVariantCustom);
            }

            var baseProductCustom = new Model_BaseProductCustom()
            {
                Id = baseProduct.Id,
                Name = baseProduct.Name,
                Description = baseProduct.Description,
                mainCategory = baseProduct.MainCategory,
                material = baseProduct.Material,
                productVariants = productVariants,
                ImagesBases = images,
                Price = baseProduct.Price,
                Discount = baseProduct.Discount,
                TotalPrice = baseProduct.TotalPrice,    
            };

            return baseProductCustom;
        }


        public static Model_BaseProductWithNoExtraInfo ConvertToDtoProductNoInfo(
            this BaseProduct baseProduct)
        {
            var baseProductNoInfo = new Model_BaseProductWithNoExtraInfo()
            {
                Id = baseProduct.Id,
                Name = baseProduct.Name,
                Description = baseProduct.Description,
                MaterialId = baseProduct.MaterialId,
                MainCategoryId = baseProduct.MainCategoryId,
                Price = baseProduct.Price,
                Discount = baseProduct.Discount,
                TotalPrice = baseProduct.TotalPrice
            };

            return baseProductNoInfo;
        }

        public static IEnumerable<Model_ProductVariantReturn> ConvertToDtoProductVariant
            (this IEnumerable<ProductVariant> productVariants)
        {
            var returnProductVariantCustom = (from productVariant in productVariants
                                              select new Model_ProductVariantReturn
                                              {
                                                  Id = productVariant.Id,
                                                  BaseProductId = productVariant.BaseProductId,
                                                  productColor = productVariant.productColor,
                                                  productSize = productVariant.productSize,
                                                  Quantity = productVariant.Quantity
                                              });
            return returnProductVariantCustom;
        }

        public static Model_ProductVariantWithoutObj ConvertToDtoWithoutObj
            (this ProductVariant productVariant)
        {
            var productVariantWithoutObj = new Model_ProductVariantWithoutObj()
            {
                Id = productVariant.Id,
                BaseProductId = productVariant.BaseProductId,
                ProductColorId = productVariant.ProductColorId,
                ProductSizeId = productVariant.ProductSizeId,
                Quantity = productVariant.Quantity
            };

            return productVariantWithoutObj;
        }

        public static Model_ProductVariantReturn ConvertToDtoWithObj
            (this ProductVariant productVariant)
        {
            var productVariantWIthObj = new Model_ProductVariantReturn()
            {
                Id = productVariant.Id,
                BaseProductId = productVariant.BaseProductId,
                productColor = productVariant.productColor,
                productSize = productVariant.productSize,
                Quantity = productVariant.Quantity
            };

            return productVariantWIthObj;
        }


       public static Model_CartItemReturn ConvertToDtoCartItem(
           this BaseProduct baseProduct, ProductVariant productVariant, CartItem cartItem)
        {
            Model_CartItemReturn cartItemReturn = new Model_CartItemReturn();
            bool enoughItems = true;

            if(productVariant.Quantity  < cartItem.Quantity)
            {
                enoughItems = false;
            }

            cartItemReturn.BaseProductId = baseProduct.Id;
            cartItemReturn.BaseProductName = baseProduct.Name;
            cartItemReturn.BaseProductDescription = baseProduct.Description;
            cartItemReturn.Price = baseProduct.Price;
            cartItemReturn.Discount = baseProduct.Discount;
            cartItemReturn.TotalPricePerItem = baseProduct.TotalPrice;
            cartItemReturn.ProductVariantId = productVariant.Id;

            Model_ProductColorCustom productColorCustom = new Model_ProductColorCustom()
            {
                Id = productVariant.productColor.Id,
                Name = productVariant.productColor.Name,
            };
            Model_ProductSizeCustom productSizeCustom = new Model_ProductSizeCustom()
            {
                Id = productVariant.productSize.Id,
                Name = productVariant.productSize.Name,
            };

            cartItemReturn.ProductColor = productColorCustom;
            cartItemReturn.ProductSize = productSizeCustom;

            cartItemReturn.AvailableQuantity = productVariant.Quantity;
            cartItemReturn.SelectedQuantity = cartItem.Quantity;
            cartItemReturn.CanBeSold = enoughItems;

            cartItemReturn.TotalPrice = cartItem.Quantity * baseProduct.TotalPrice;

            if(enoughItems)
            {
                cartItemReturn.Message = "Item Can Be Sold";
            }
            else
            {
                cartItemReturn.Message = "Not Enough Items In Storage";
            }

            return cartItemReturn;
        }


        public static Response_AllExchangedGoodItems ConvertToDtoGoodItems(
            this ItemExchangeRequest itemExchangeRequest)
        {
            Response_AllExchangedGoodItems allExchangedGoodItems = new Response_AllExchangedGoodItems();

            allExchangedGoodItems.OrderUniqueIdentifier = itemExchangeRequest.OrderUniqueIdentifier;
            allExchangedGoodItems.ExchangeUniqueIdentfier = itemExchangeRequest.ExchangeUniqueIdentifier;

            List<ExchangeGoodItem> exchangeGoodItems = new List<ExchangeGoodItem>();

            foreach(var item in itemExchangeRequest.exchangeOrderItems)
            {
                ExchangeGoodItem newExchangeGoodItem = new ExchangeGoodItem();

                newExchangeGoodItem.Id = item.Id;
                newExchangeGoodItem.BaseProductId = item.BaseProductId;
                newExchangeGoodItem.BaseProductName = item.BaseProductName;
                newExchangeGoodItem.ReturnedProductVariantId = item.ReturnedProductVariantId;
                newExchangeGoodItem.ReturnedProductVariantColor = item.ReturnedProductVariantColor;
                newExchangeGoodItem.ReturnedProductVariantSize = item.ReturnedProductVariantSize;
                newExchangeGoodItem.ExchangedProductVariantId = item.ExchangedProductVariantId;
                newExchangeGoodItem.ExchangedProductVariantColor = item.ExchangedProductVariantColor;
                newExchangeGoodItem.ExchangedProductVariantSize = item.ExchangedProductVariantSize;
                newExchangeGoodItem.Quantity = item.Quantity;
                newExchangeGoodItem.Message = item.Message;

                exchangeGoodItems.Add(newExchangeGoodItem);
            }

            allExchangedGoodItems.exchangeGoodItems = exchangeGoodItems;

            return allExchangedGoodItems;
        }

        public static Response_AllExchangePendingItems ConverToDtoPendingItems(
            this ItemExchangeRequest itemExchangeRequest)
        {
            Response_AllExchangePendingItems allExchangePendingItems =
                new Response_AllExchangePendingItems();

            allExchangePendingItems.OrderUniqueIdentifier =
                itemExchangeRequest.OrderUniqueIdentifier;

            allExchangePendingItems.ExchangeUniqueIdentifier =
                itemExchangeRequest.ExchangeUniqueIdentifier;

            List<DTOs.RequestExchangeDtos.Models.ExchangeItemPendingDTO> exchangeItemPendingList = 
                new List<DTOs.RequestExchangeDtos.Models.ExchangeItemPendingDTO>();

            foreach(var item in itemExchangeRequest.exchangeItemsPending)
            {
                DTOs.RequestExchangeDtos.Models.ExchangeItemPendingDTO exchangeItemPending 
                    = new();

                exchangeItemPending.Id = item.Id;
                exchangeItemPending.BaseProductId = item.BaseProductId;
                exchangeItemPending.BaseProductName = item.BaseProductName;
                exchangeItemPending.RetrunedProductVariantId = item.ReturnedProductVariantId;
                exchangeItemPending.RetrunedProductVariantColor = 
                    item.ReturnedProductVariantColor;
                exchangeItemPending.ReturnedProductVariantSize = 
                    item.ReturnedProductVariantSize;
                exchangeItemPending.Quantity = item.Quantity;
                exchangeItemPending.Message = item.Message;

                exchangeItemPending.PricePerItemPaid = item.PricePerItemPaid;

                exchangeItemPendingList.Add(exchangeItemPending);
            }

            allExchangePendingItems.ExchangeItemsPending = exchangeItemPendingList;

            return allExchangePendingItems;
        }

        public static Response_AllExchangeBadItems ConvertToDtoBadItems
            (this ItemExchangeRequest itemExchangeRequest)
        {
            Response_AllExchangeBadItems response_AllExchangeBadItems = 
                new Response_AllExchangeBadItems();

            response_AllExchangeBadItems.OrderUniqueIdentifier =
                itemExchangeRequest.OrderUniqueIdentifier;
            response_AllExchangeBadItems.ExchangeUniqueIdentifier =
                itemExchangeRequest.ExchangeUniqueIdentifier;

            List<ExchangeBadItem> exchangeBadItems = new List<ExchangeBadItem>();

            foreach(var item in itemExchangeRequest.exchangeItemsCanceled)
            {
                ExchangeBadItem exchangeBadItem = new ExchangeBadItem();
                exchangeBadItem.Id = item.Id;
                exchangeBadItem.BaseProductId  = item.BaseProductId;
                exchangeBadItem.BaseProductName = item.BaseProductName;
                exchangeBadItem.ReturnedProductVariantId = item.ReturnedProductVariantId;

                exchangeBadItem.ReturnedProductVariantColor = 
                    item.ReturnedProductVariantColor;
                exchangeBadItem.ReturnedProductVariantSize = item.ReturnedProductVariantSize;
                exchangeBadItem.Quantity = item.Quantity;
                exchangeBadItem.CanceliationReason = item.CancelationReason;

                exchangeBadItems.Add(exchangeBadItem);
            }
            response_AllExchangeBadItems.exchangeBadItems = exchangeBadItems;
            return response_AllExchangeBadItems;

        }

    }

}
