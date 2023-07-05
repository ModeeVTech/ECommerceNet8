using AutoMapper;
using ECommerceNet8.Data;
using ECommerceNet8.DTOs.ProductDtos.Request;
using ECommerceNet8.DTOs.ProductDtos.Response;
using ECommerceNet8.Models.ProductModels;
using Microsoft.EntityFrameworkCore;

namespace ECommerceNet8.Repositories.MaterialRepository
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public MaterialRepository(ApplicationDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<Response_ProductMaterial> GetAllProductMaterials()
        {
            var productMaterials = await _db.Materials.ToListAsync();
            if(productMaterials.Count <= 0)
            {
                return new Response_ProductMaterial()
                {
                    isSuccess = false,
                    Message = "No product materials found"
                };
            }
            return new Response_ProductMaterial()
            {
                isSuccess = true,
                Message = "All product materials",
                materials = productMaterials
            };
        }

        public async Task<Response_ProductMaterial> GetProductMaterialById(int productMaterialId)
        {
            var productMaterial = await _db.Materials
                .FirstOrDefaultAsync(m => m.Id == productMaterialId);

            if(productMaterial == null)
            {
                return new Response_ProductMaterial()
                {
                    isSuccess = false,
                    Message = "No product materials found with given Id"
                };
            }

            return new Response_ProductMaterial()
            {
                isSuccess = true,
                Message = "Material retrieved successfully",
                materials = new List<Material>()
                {
                    productMaterial
                }
            };
        }
        public async Task<Response_ProductMaterial> AddProductMaterial(Request_ProductMaterial productMaterialDto)
        {
            var productMaterialBase = _mapper.Map<Material>(productMaterialDto);

            await _db.Materials.AddAsync(productMaterialBase);
            await _db.SaveChangesAsync();

            return new Response_ProductMaterial()
            {
                isSuccess = true,
                Message = "Product Material Added Successfully",
                materials = new List<Material>()
                {
                    productMaterialBase
                }
            };
        }

        public async Task<Response_ProductMaterial> UpdateProductMaterial(int productMaterialId, Request_ProductMaterial productMaterialDto)
        {
            var ExistingProductMaterial = await _db.Materials
                .FirstOrDefaultAsync(m=>m.Id == productMaterialId);

            if(ExistingProductMaterial == null)
            {
                return new Response_ProductMaterial()
                {
                    isSuccess = false,
                    Message = "No product material found with given Id"
                };
            }
            ExistingProductMaterial.Name = productMaterialDto.Name;
            await _db.SaveChangesAsync();

            return new Response_ProductMaterial()
            {
                isSuccess = true,
                Message = "Product Material Updated Successfully",
                materials = new List<Material>()
                {
                    ExistingProductMaterial
                }
            };
        }
        public async Task<Response_ProductMaterial> DeleteProductMaterial(int productMaterialId)
        {
            var existingProductMaterial = await _db.Materials
                .FirstOrDefaultAsync(m=>m.Id==productMaterialId);
            if(existingProductMaterial == null)
            {
                return new Response_ProductMaterial()
                {
                    isSuccess = false,
                    Message = "No product material found with given Id"
                };
            }

            _db.Materials.Remove(existingProductMaterial);
            await _db.SaveChangesAsync();

            return new Response_ProductMaterial()
            {
                isSuccess = true,
                Message = "Product Material Removed Successfully",
                materials = new List<Material>()
                {
                    existingProductMaterial
                }
            };
        }

    }
}
