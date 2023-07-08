using AutoMapper;
using ECommerceNet8.Data;
using ECommerceNet8.DTOs.ProducSizeDtos.Request;
using ECommerceNet8.DTOs.ProducSizeDtos.Response;
using ECommerceNet8.Models.ProductModels;
using Microsoft.EntityFrameworkCore;

namespace ECommerceNet8.Repositories.ProductSizeRepository
{
    public class ProductSizeRepository : IProductSizeRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public ProductSizeRepository(ApplicationDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


        public async Task<Response_ProductSize> GetAllProductSizes()
        {
            var productSizes = await _db.ProductSizes.ToListAsync();
            if(productSizes == null)
            {
                return new Response_ProductSize()
                {
                    isSuccess = false,
                    Message = "No product sizes found"
                };
            }
            return new Response_ProductSize()
            {
                isSuccess = true,
                Message = "All product sizes retrieved",
                ProductSizes = productSizes
            };
        }

        public async Task<Response_ProductSize> GetProductSizeById(int productSizeId)
        {
            var productSize = await  _db.ProductSizes
                .FirstOrDefaultAsync(ps=>ps.Id == productSizeId);
            if(productSize == null)
            {
                return new Response_ProductSize()
                {
                    isSuccess = false,
                    Message = "No product size found with given Id"
                };
            }
            return new Response_ProductSize()
            {
                isSuccess = true,
                Message = "Size retrieved successfully",
                ProductSizes = new List<ProductSize>()
                {
                    productSize
                }
            };
        }
        public async Task<Response_ProductSize> AddProductSize(Request_ProductSize productSize)
        {
            var produtSizeBase = _mapper.Map<ProductSize>(productSize);

            await _db.ProductSizes.AddAsync(produtSizeBase);
            await _db.SaveChangesAsync();

            return new Response_ProductSize()
            {
                isSuccess = true,
                Message = "Product Size Added",
                ProductSizes = new List<ProductSize>()
                {
                    produtSizeBase
                }
            };

        }
        public async Task<Response_ProductSize> UpdateProductSize(int productSizeId, Request_ProductSize productSize)
        {
            var existingProductSize = await _db.ProductSizes
                .FirstOrDefaultAsync(ps=>ps.Id==productSizeId);
            if(existingProductSize == null)
            {
                return new Response_ProductSize()
                {
                    isSuccess = false,
                    Message = "No product size found with given Id"
                };
            }
            existingProductSize.Name = productSize.Name;
            await _db.SaveChangesAsync();

            return new Response_ProductSize()
            {
                isSuccess = true,
                Message = "Product size updated",
                ProductSizes = new List<ProductSize>()
                {
                    existingProductSize
                }
            };
        }
        public async Task<Response_ProductSize> DeleteProductSize(int productSizeId)
        {
            var existingProductSize = await _db.ProductSizes
                .FirstOrDefaultAsync(ps=>ps.Id == productSizeId);
            if(existingProductSize == null)
            {
                return new Response_ProductSize()
                {
                    isSuccess = false,
                    Message = "No product size found with given Id"
                };
            }

            _db.ProductSizes.Remove(existingProductSize);
            await _db.SaveChangesAsync();

            return new Response_ProductSize()
            {
                isSuccess = true,
                Message = "Product size Removed Successfully",
                ProductSizes = new List<ProductSize>()
                {
                    existingProductSize
                }
            };
        }
    }
}
