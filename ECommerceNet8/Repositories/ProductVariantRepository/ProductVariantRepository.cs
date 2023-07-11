using AutoMapper;
using ECommerceNet8.Data;
using ECommerceNet8.DTOConvertions;
using ECommerceNet8.DTOs.ProductVariantDtos.CustomModels;
using ECommerceNet8.DTOs.ProductVariantDtos.Request;
using ECommerceNet8.DTOs.ProductVariantDtos.Response;
using ECommerceNet8.Models.ProductModels;
using Microsoft.EntityFrameworkCore;

namespace ECommerceNet8.Repositories.ProductVariantRepository
{
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ProductVariantRepository(ApplicationDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<Response_ProductVariantWithObj> GetAllVariantsByBaseProductId(int baseProductId)
        {
            var productVariantBase = await _db.ProductVariants
                .Where(pv => pv.BaseProductId == baseProductId)
                .Include(pv => pv.productColor)
                .Include(pv => pv.productSize)
                .ToListAsync();

            if(productVariantBase == null) 
            {
                return new Response_ProductVariantWithObj()
                {
                    isSuccess = false,
                    Message = "No Product Variants Found With Given Base Product Id"
                };
            }

            //CONVERT TO DTO
            var productVariantReturn = productVariantBase.ConvertToDtoProductVariant()
                .ToList();

            return new Response_ProductVariantWithObj()
            {
                isSuccess = true,
                Message = " All Product Variants Retrieved",
                ProductVariants = productVariantReturn
            };
        }

        public async Task<Response_ProductVariantWithObj> GetProductVariantById(int productVariantId)
        {
            var productVariantBase = await  _db.ProductVariants
                .Include(pv=>pv.productColor)
                .Include(pv=>pv.productSize)
                .FirstOrDefaultAsync(pv=>pv.Id == productVariantId);

            if(productVariantBase == null)
            {
                return new Response_ProductVariantWithObj()
                {
                    isSuccess = false,
                    Message = "No Product Variants Found With Given Id"
                };
            }

            //MAP
            var productVariantReturnObj = _mapper.Map<Model_ProductVariantReturn>(productVariantBase);

            return new Response_ProductVariantWithObj()
            {
                isSuccess = true,
                Message = "Product Variant Retrieved",
                ProductVariants = new List<Model_ProductVariantReturn>()
                {
                    productVariantReturnObj
                }
            };
        }


        public async Task<ProductVariant> GetVariantForValidations(int productVariantId)
        {
            var existingProductVariant  = await _db.ProductVariants
                .FirstOrDefaultAsync(pv=>pv.Id == productVariantId);

            return existingProductVariant;
        }

        public async Task<Response_ProductVariantWithoutObj> AddProductVariant(Model_ProductVariantRequest productVariantAdd)
        {
            //MAP
            var productVariantBase = _mapper.Map<ProductVariant>(productVariantAdd);

            await _db.ProductVariants.AddAsync(productVariantBase);
            await _db.SaveChangesAsync();

            //CONVERT TO DTO
            var productVariantWithoutObj = productVariantBase.ConvertToDtoWithoutObj();

            return new Response_ProductVariantWithoutObj()
            {
                isSuccess = true,
                Message = "Product Variant Added",
                ProductVariantWithoutObj = new List<Model_ProductVariantWithoutObj>()
                {
                    productVariantWithoutObj
                }
            };
        }

        public async Task<Response_ProductVariantWithoutObj> UpdateProductVariant(int productVariantId, Model_ProductVariantRequest productVariantUpdate)
        {
            var existingProductVariant = await _db.ProductVariants
                .FirstOrDefaultAsync(pv => pv.Id == productVariantId);
            if(existingProductVariant == null)
            {
                return new Response_ProductVariantWithoutObj()
                {
                    isSuccess = false,
                    Message = "No Product Variant Found With Given Id"
                };
            }

            existingProductVariant.BaseProductId = productVariantUpdate.BaseProductId;
            existingProductVariant.ProductColorId = productVariantUpdate.ProductColorId;
            existingProductVariant.ProductSizeId  = productVariantUpdate.ProductSizeId;
            existingProductVariant.Quantity = productVariantUpdate.Quantity;
            await _db.SaveChangesAsync();

            //CONVERT TO DTO
            var productVariantWithoutObj = existingProductVariant.ConvertToDtoWithoutObj();

            return new Response_ProductVariantWithoutObj()
            {
                isSuccess = true,
                Message = "Product Variant Updated Successfully",
                ProductVariantWithoutObj = new List<Model_ProductVariantWithoutObj>()
                {
                    productVariantWithoutObj
                }
            };
        }


        public async Task<Response_ProductVariantWithoutObj> UpdateProductVariantBaseProduct(int productVariantId, Request_ProductVariantUpdateBase productVariantUpdateBase)
        {
            var existingProductVariant = await _db.ProductVariants
                .FirstOrDefaultAsync(pv=>pv.Id == productVariantId);
            if(existingProductVariant == null)
            {
                return new Response_ProductVariantWithoutObj()
                {
                    isSuccess = false,
                    Message = "No Product Variant Found With Given Id"
                };
            }
            existingProductVariant.BaseProductId = productVariantUpdateBase.BaseProductId;
            await _db.SaveChangesAsync();

            //CONVERT TO DTO
            var productVariantWithoutObj = existingProductVariant.ConvertToDtoWithoutObj();

            return new Response_ProductVariantWithoutObj()
            {
                isSuccess = true,
                Message = "Product Variant Base Product Updated Successfully",
                ProductVariantWithoutObj = new List<Model_ProductVariantWithoutObj>()
                {
                    productVariantWithoutObj
                }
            };
        }

        public async Task<Response_ProductVariantWithoutObj> UpdateProductVariantColor(int productVariantId, Request_ProductVariantUpdateColor productVariantUpdateColor)
        {
            var existingProductVariant = await _db.ProductVariants
                .FirstOrDefaultAsync(pv=>pv.Id == productVariantId);
            if(existingProductVariant == null)
            {
                return new Response_ProductVariantWithoutObj()
                {
                    isSuccess = false,
                    Message = "No Product Variant Found With Given Id"
                };
            }
            existingProductVariant.ProductColorId = productVariantUpdateColor.ColorId;
            await _db.SaveChangesAsync();

            //CONVERT TO DTO
            var productVariantWithoutObj = existingProductVariant.ConvertToDtoWithoutObj();

            return new Response_ProductVariantWithoutObj()
            {
                isSuccess = true,
                Message = "Product Variant Color Updated",
                ProductVariantWithoutObj = new List<Model_ProductVariantWithoutObj>()
                {
                    productVariantWithoutObj
                }
            };
        }

        public async Task<Response_ProductVariantWithoutObj> UpdateProductVariantSize(int productVariantId, Request_ProductVariantUpdateSize productVariantUpdateSize)
        {
            var existingProductVariant = await  _db.ProductVariants
                .FirstOrDefaultAsync(pv=>pv.Id == productVariantId);
            if(existingProductVariant == null)
            {
                return new Response_ProductVariantWithoutObj()
                {
                    isSuccess = false,
                    Message = "No Product Variant Found With Given Id"
                };
            }

            existingProductVariant.ProductSizeId = productVariantUpdateSize.SizeId;
            await _db.SaveChangesAsync();

            //CONVERT TO DTO
            var productVariantWithoutObj = existingProductVariant.ConvertToDtoWithoutObj();

            return new Response_ProductVariantWithoutObj()
            {
                isSuccess = true,
                Message = "Product Variant Updated Successfully",
                ProductVariantWithoutObj = new List<Model_ProductVariantWithoutObj>()
                {
                    productVariantWithoutObj
                }
            };
        }


        public async Task<Response_ProductVariantWithoutObj> DeleteProductVariant(int productVariantId)
        {
            var existigProductVariant = await _db.ProductVariants
                .FirstOrDefaultAsync(pv => pv.Id == productVariantId);
            if(existigProductVariant == null)
            {
                return new Response_ProductVariantWithoutObj()
                {
                    isSuccess = false,
                    Message = "No Product Variant Found With Given Id"
                };
            }
            _db.ProductVariants.Remove(existigProductVariant);
            await _db.SaveChangesAsync();

            //CONVERT TO DTO
            var productVariantWithoutObj = existigProductVariant.ConvertToDtoWithoutObj();

            return new Response_ProductVariantWithoutObj()
            {
                isSuccess = true,
                Message = "Product Variant Removed Successfully",
                ProductVariantWithoutObj = new List<Model_ProductVariantWithoutObj>()
                {
                    productVariantWithoutObj
                }
            };
        }

        public Task<Response_ProductVariantWithoutObj> AddQuantity(int productVariantId, int quanity)
        {
            throw new NotImplementedException();
        }




        

        public Task<Response_ProductVariantWithObj> GetProductVariantSelection(int productBaseId, int colorId, int sizeId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Response_ProductVariantSizes>> GetProductVariantSizes(int productBaseId, int colorId)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<Response_ProductVariantCheckQty>> HasEnoughItems(Request_ProductVariantCheck productVariantToCheck)
        {
            throw new NotImplementedException();
        }

        public Task<Response_ProductVariantWithoutObj> RemoveQuantity(int productVariantId, int quantity)
        {
            throw new NotImplementedException();
        }






    }
}
