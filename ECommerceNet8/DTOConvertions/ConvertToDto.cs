using ECommerceNet8.DTOs.BaseProductDtos.CustomModels;
using ECommerceNet8.Models.ProductModels;

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

    }

}
