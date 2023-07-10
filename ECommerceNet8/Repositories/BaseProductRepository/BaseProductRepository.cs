using ECommerceNet8.Data;
using ECommerceNet8.DTOConvertions;
using ECommerceNet8.DTOs.BaseProductDtos.CustomModels;
using ECommerceNet8.DTOs.BaseProductDtos.Request;
using ECommerceNet8.DTOs.BaseProductDtos.Response;
using ECommerceNet8.Models.ProductModels;
using Microsoft.EntityFrameworkCore;

namespace ECommerceNet8.Repositories.BaseProductRepository
{
    public class BaseProductRepository : IBaseProductRepository
    {
        private readonly ApplicationDbContext _db;

        public BaseProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<IEnumerable<BaseProduct>> GetAllAsync()
        {
            var baseProducts = await _db.BaseProducts
                .Include(bp => bp.MainCategory)
                .Include(bp => bp.Material)
                .Include(bp => bp.productVariants).ThenInclude(pv => pv.productColor)
                .Include(bp => bp.productVariants).ThenInclude(pv => pv.productSize)
                .Include(bp => bp.imageBases)
                .ToListAsync();

            return baseProducts;
        }

        public async Task<IEnumerable<Model_BaseProductCustom>> GetAllWithFullInfoAsync()
        {
            var baseProducts = await _db.BaseProducts
                .Include(bp => bp.MainCategory)
                .Include(bp => bp.Material)
                .Include(bp => bp.productVariants).ThenInclude(pv => pv.productColor)
                .Include(bp => bp.productVariants).ThenInclude(pv => pv.productSize)
                .Include(bp => bp.imageBases)
                .ToListAsync();

            //CONVERT TO DTO
            var CustomBaseProduct = baseProducts.ConvertToDtoListCustomProduct();

            return CustomBaseProduct;
        }

        public async Task<Response_BaseProductWithPaging> GetAllWithFullInfoByPages(int pageNumber, int pageSize)
        {
            Response_BaseProductWithPaging baseProductWithPaging =
                new Response_BaseProductWithPaging();

            float numberpp = (float)pageSize;
            var totalPages = Math.Ceiling((await GetAllAsync()).Count() / numberpp);
            int totPages = (int)totalPages;

            var baseProducts = await _db.BaseProducts
                .Include(bp => bp.MainCategory)
                .Include(bp => bp.Material)
                .Include(bp => bp.productVariants).ThenInclude(pv => pv.productColor)
                .Include(bp => bp.productVariants).ThenInclude(pv => pv.productSize)
                .Include(bp => bp.imageBases)
                .Skip((pageNumber - 1)*pageSize)
                .Take(pageSize)
                .ToListAsync();

            //CONVERT TO DTO
            var CustomBaseProducts = baseProducts.ConvertToDtoListCustomProduct();

            baseProductWithPaging.baseProducts = CustomBaseProducts.ToList();
            baseProductWithPaging.TotalPages = totPages;

            return baseProductWithPaging;
        }

        public async Task<Response_BaseProductWithFullInfo> GetByIdWithFullInfo(int baseProductId)
        {
            var existingBaseProduct = await _db.BaseProducts
                .Include(bp => bp.MainCategory)
                .Include(bp => bp.Material)
                .Include(bp => bp.productVariants).ThenInclude(pv => pv.productColor)
                .Include(bp => bp.productVariants).ThenInclude(pv => pv.productSize)
                .Include(bp => bp.imageBases)
                .FirstOrDefaultAsync(bp=>bp.Id == baseProductId);

            if(existingBaseProduct == null)
            {
                return new Response_BaseProductWithFullInfo()
                {
                    isSuccess = false,
                    Message = "No Base Product with given Id"
                };
            }
            //CONVERT TO DTO

            var baseProductCustom = existingBaseProduct.ConvertToDtoCustomProduct();

            return new Response_BaseProductWithFullInfo()
            {
                isSuccess = true,
                Message = "Base product retrieved",
                baseProduct = baseProductCustom
            };
        }


        public async Task<Response_BaseProduct> GetByIdWithNoInfo(int baseProductId)
        {
            Response_BaseProduct baseProductResponse = new Response_BaseProduct();

            var existingBaseProduct = await _db.BaseProducts
                .FirstOrDefaultAsync(bp=>bp.Id==baseProductId);

            if(existingBaseProduct == null)
            {
                baseProductResponse.isSuccess = false;
                baseProductResponse.Message = "No Base Product Found With Given Id";

                return baseProductResponse;
            }

            //CONVENRT TO DTO
            var baseProductWithNoInfo = existingBaseProduct.ConvertToDtoProductNoInfo();

            baseProductResponse.isSuccess = true;
            baseProductResponse.Message = "Base Product Found";
            baseProductResponse.baseProducts.Add(baseProductWithNoInfo);

            return baseProductResponse;
        }

        public async Task<Response_BaseProduct> AddBaseProduct(Request_BaseProduct baseProduct)
        {
            //CONVERT TO BASE

            var baseProductDB = baseProduct.ConvertToBaseProduct();

            await _db.BaseProducts.AddAsync(baseProductDB);
            await _db.SaveChangesAsync();

            //CONVERT TO DTO
            var baseProdutWithNoInfo = baseProductDB.ConvertToDtoProductNoInfo();

            return new Response_BaseProduct()
            {
                isSuccess = true,
                Message = "Base Product Added Successfully",
                baseProducts = new List<Model_BaseProductWithNoExtraInfo>()
                {
                    baseProdutWithNoInfo
                }
            };

        }


        public async Task<Response_BaseProduct> UpdateBaseProduct(int baseProductId, Request_BaseProduct baseProduct)
        {
            var exsitingBaseProduct = await _db.BaseProducts
                .FirstOrDefaultAsync(bp=>bp.Id == baseProductId);
            if(exsitingBaseProduct == null)
            {
                return new Response_BaseProduct()
                {
                    isSuccess = false,
                    Message = "No base product found with given Id"
                };
            }

            exsitingBaseProduct.Name = baseProduct.Name;
            exsitingBaseProduct.Description = baseProduct.Description;
            exsitingBaseProduct.MainCategoryId = baseProduct.MainCategoryId;
            exsitingBaseProduct.MaterialId = baseProduct.MaterialId;
            exsitingBaseProduct.Price = baseProduct.Price;
            exsitingBaseProduct.Discount = baseProduct.Discount;

            //CALCULATE TOTAL PRICE
            var totalPrice = (exsitingBaseProduct.Price - (exsitingBaseProduct.Price
                * exsitingBaseProduct.Discount / 100));
            var totalPriceDecimal = decimal.Round(totalPrice, 2);
            exsitingBaseProduct.TotalPrice = totalPriceDecimal;

            await _db.SaveChangesAsync();

            var baseProductWithNoInfo = exsitingBaseProduct.ConvertToDtoProductNoInfo();

            return new Response_BaseProduct()
            {
                isSuccess = true,
                Message = "Base product updated",
                baseProducts = new List<Model_BaseProductWithNoExtraInfo>()
                {
                    baseProductWithNoInfo
                }
            };

        }

        public async Task<Response_BaseProduct> UpdateBaseProductDiscount(int baseProductId, Request_BaseProductDiscount baseProductDiscount)
        {
            var existingBaseProduct = await _db.BaseProducts
                .FirstOrDefaultAsync(bp=>bp.Id == baseProductId);
            if(existingBaseProduct == null)
            {
                return new Response_BaseProduct()
                {
                    isSuccess = false,
                    Message = "No base product found with given Id"
                };
            }

            if(baseProductDiscount.Discount > 99 ||
                baseProductDiscount.Discount < 0)
            {
                return new Response_BaseProduct()
                {
                    isSuccess = false,
                    Message = "Discount amount is wrong"
                };
            }

            existingBaseProduct.Discount = baseProductDiscount.Discount;

            //CALCULATE TOTAL PRICE
            decimal totalPrice;
            decimal totalPriceDecimal;

            if(baseProductDiscount.Discount == 0)
            {
                totalPrice = existingBaseProduct.Price;
                totalPriceDecimal = decimal.Round(totalPrice, 2);
            }
            else
            {
                totalPrice = existingBaseProduct.Price - (existingBaseProduct.Price
                    * baseProductDiscount.Discount / 100);
                totalPriceDecimal = decimal.Round(totalPrice, 2);
            }

            existingBaseProduct.TotalPrice = totalPriceDecimal;
            await _db.SaveChangesAsync();

            var baseProductWithNoInfo = existingBaseProduct.ConvertToDtoProductNoInfo();

            return new Response_BaseProduct()
            {
                isSuccess = true,
                Message = "Product Discount Updated Successfully",
                baseProducts = new List<Model_BaseProductWithNoExtraInfo>()
                {
                    baseProductWithNoInfo
                }
            };
            
        }

        public async Task<Response_BaseProduct> UpdateBaseProductPrice(int baseProductId, Request_BaseProductPrice baseProductPrice)
        {
            var existingBaseProduct = await _db.BaseProducts
                .FirstOrDefaultAsync(bp => bp.Id == baseProductId);
            if(existingBaseProduct == null)
            {
                return new Response_BaseProduct()
                {
                    isSuccess = false,
                    Message = "No base product found with given Id"
                };
            }

            existingBaseProduct.Price = baseProductPrice.Price;


            decimal totalPriceCalculated;
            //RECALCULATE TOTAL PRICE
            if(existingBaseProduct.Discount == 0)
            {
                var totalPrice = baseProductPrice.Price;
                totalPriceCalculated = decimal.Round(totalPrice, 2);
            }
            else
            {
                var totalPrice = baseProductPrice.Price - (baseProductPrice.Price
                    * existingBaseProduct.Discount / 100);
                totalPriceCalculated = decimal.Round(totalPrice, 2);
            }

            existingBaseProduct.TotalPrice = totalPriceCalculated;
            await _db.SaveChangesAsync();

            var baseProductWithNoInfo = existingBaseProduct.ConvertToDtoProductNoInfo();

            return new Response_BaseProduct()
            {
                isSuccess = true,
                Message = "Base Product Price Updated",
                baseProducts = new List<Model_BaseProductWithNoExtraInfo>()
                {
                    baseProductWithNoInfo
                }
            };         
        }

        public async Task<Response_BaseProduct> UpdateBaseProductMainCategory(int baseProductId, Request_BaseProductMainCategory baseProductMainCategory)
        {
            var existingBaseProduct = await _db.BaseProducts
                .FirstOrDefaultAsync(bp => bp.Id == baseProductId);

            if(existingBaseProduct == null)
            {
                return new Response_BaseProduct()
                {
                    isSuccess = false,
                    Message = "No Base product found with given Id"
                };
            }

            existingBaseProduct.MainCategoryId = baseProductMainCategory.MainCategoryId;
            await _db.SaveChangesAsync();

            var baseProductWithNoInfo = existingBaseProduct.ConvertToDtoProductNoInfo();

            return new Response_BaseProduct()
            {
                isSuccess = true,
                Message = "Main Category Updated",
                baseProducts = new List<Model_BaseProductWithNoExtraInfo>
                {
                    baseProductWithNoInfo
                }
            };

        }

        public async Task<Response_BaseProduct> UpdateBaseProductMaterial(int baseProductId, Request_BaseProductMaterial baseProductMaterial)
        {
            var existingBaseProduct = await _db.BaseProducts
                .FirstOrDefaultAsync(bp=>bp.Id == baseProductId);
            if(existingBaseProduct == null)
            {
                return new Response_BaseProduct()
                {
                    isSuccess = false,
                    Message = "No base product found with given Id"
                };
            }

            existingBaseProduct.MaterialId = baseProductMaterial.MaterialId;
            await _db.SaveChangesAsync();

            var baseProductWithNoInfo = existingBaseProduct.ConvertToDtoProductNoInfo();

            return new Response_BaseProduct()
            {
                isSuccess = true,
                Message = "Material Id Updated Successfully",
                baseProducts = new List<Model_BaseProductWithNoExtraInfo>()
                {
                    baseProductWithNoInfo
                }
            };
        }
        public async Task<Response_BaseProduct> RemoveBaseProduct(int baseProductId)
        {
            var existingBaseProduct = await _db.BaseProducts
                .FirstOrDefaultAsync(bp=>bp.Id == baseProductId);
            if(existingBaseProduct == null)
            {
                return new Response_BaseProduct()
                {
                    isSuccess = false,
                    Message = "No Base Product Found With Given Id"
                };
            }

            _db.BaseProducts.Remove(existingBaseProduct);
            await _db.SaveChangesAsync();

            var baseProductWithNoInfo = existingBaseProduct.ConvertToDtoProductNoInfo();

            return new Response_BaseProduct()
            {
                isSuccess = true,
                Message = "Base product Removed Successfully",
                baseProducts = new List<Model_BaseProductWithNoExtraInfo>()
                {
                    baseProductWithNoInfo
                }
            };
        }

        public async Task<IEnumerable<string>> GetProductSearchSuggestions(string searchText)
        {
            var products = await FindProductBySearchText(searchText);

            List<string> searchResult = new List<string>();

            foreach (var product in products)
            {
                if(product.Name.Contains(searchText,StringComparison.OrdinalIgnoreCase))
                {
                    searchResult.Add(product.Name);
                }
                //search match in description
                if(product.Description != null)
                {
                    var punctuation = product.Description.Where(char.IsPunctuation)
                        .Distinct().ToArray();

                    var words = product.Description.Split()
                        .Select(w => w.Trim(punctuation));

                    foreach(var word in words)
                    {
                        if(word.Contains(searchText,StringComparison.OrdinalIgnoreCase)
                            && !searchResult.Contains(word))
                        {
                            searchResult.Add(word);     
                        }
                    }
                }
            }

            return searchResult;
        }

        public async Task<IEnumerable<Model_BaseProductCustom>> GetProductSearch(string searchText)
        {
            var baseProducts = await _db.BaseProducts
                .Where(bp => bp.Name.ToLower().Contains(searchText.ToLower())
                || bp.Description.Contains(searchText.ToLower()))
                .Include(bp => bp.MainCategory)
                .Include(bp => bp.Material)
                .Include(bp => bp.productVariants).ThenInclude(pv => pv.productColor)
                .Include(bp => bp.productVariants).ThenInclude(pv => pv.productSize)
                .Include(bp => bp.imageBases)
                .ToListAsync();

            if(baseProducts == null)
            {
                return null;
            }

            var baseProductCustomReturn = baseProducts.ConvertToDtoListCustomProduct();

            return baseProductCustomReturn;
        }

 

        public async Task<Response_BaseProductWithPaging> GetProductSearchWithPaging(string searchText, int pageNumber, int pageSize)
        {
            float numberpp = (float)pageSize;
            float currPage = (float)pageNumber;
            var totPages = Math.Ceiling(
                (await GetProductSearch(searchText)).Count() / numberpp);
            var totalPages = (int)totPages;

            var baseProducts = await _db.BaseProducts
                .Where(bp=> bp.Name.ToLower().Contains(searchText.ToLower()))
                .Include(bp=>bp.MainCategory)
                .Include(bp=>bp.Material)
                .Include(bp=>bp.productVariants).ThenInclude(pv => pv.productColor)
                .Include(bp=>bp.productVariants).ThenInclude(pv=>pv.productSize)
                .Include(bp=>bp.imageBases)
                .Skip(((int)pageNumber-1)* pageSize)
                .Take(pageSize)
                .ToListAsync();

            if(baseProducts == null)
            {
                return null;
            }

            var baseProductsDTO =  baseProducts.ConvertToDtoListCustomProduct();

            return new Response_BaseProductWithPaging()
            {
                TotalPages = totalPages,
                baseProducts = baseProductsDTO.ToList()
            };
        }



        public async Task<IEnumerable<Model_BaseProductCustom>> SearchProducts(int[] MaterialsIds, int[] mainCategoryIds, int[] productColorIds, int[] productSizeIds)
        {
            IQueryable<BaseProduct> queryBaseProducts = _db.BaseProducts
                .Include(bp => bp.MainCategory)
                .Include(bp => bp.Material)
                .Include(bp => bp.productVariants).ThenInclude(pv => pv.productColor)
                .Include(bp => bp.productVariants).ThenInclude(pv => pv.productSize)
                .Include(bp => bp.imageBases);

            if(MaterialsIds.Length > 0)
            {
                queryBaseProducts = queryBaseProducts
                    .Where(bp => MaterialsIds.Contains(bp.MaterialId));
            }
            if(mainCategoryIds.Length > 0)
            {
                queryBaseProducts = queryBaseProducts
                    .Where(bp => mainCategoryIds.Contains(bp.MainCategoryId));
            }

            if(productColorIds.Length > 0)
            {
                queryBaseProducts = queryBaseProducts
                    .Where(bp => bp.productVariants.ToList()
                    .Any(pv => productColorIds.Contains(pv.ProductColorId)));
            }
            if(productSizeIds.Length > 0)
            {
                queryBaseProducts = queryBaseProducts
                    .Where(bp => bp.productVariants.ToList()
                    .Any(pv => productSizeIds.Contains(pv.ProductSizeId)));
            }

            List<BaseProduct> result = await queryBaseProducts.ToListAsync();
            var baseProductCustom = result.ConvertToDtoListCustomProduct();

            return baseProductCustom;
        }




        //PRIVATE FUNCTIONS
        private async Task<IEnumerable<BaseProduct>> FindProductBySearchText(string searchText)
        {
            return await _db.BaseProducts
                .Where(bp=>bp.Name.ToLower().Contains(searchText.ToLower())
                    ||bp.Description.ToLower().Contains(searchText.ToLower())
                )
                .ToListAsync();
        }



 
    }
}
