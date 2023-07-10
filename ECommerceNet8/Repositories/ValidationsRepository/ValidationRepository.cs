using ECommerceNet8.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceNet8.Repositories.ValidationsRepository
{
    public class ValidationRepository : IValidationRepository
    {
        private readonly ApplicationDbContext _db;

        public ValidationRepository(ApplicationDbContext db)
        {
            _db = db;
        }



        public async Task<bool> ValidateMaterial(string materialName)
        {
            var existingMaterial = await _db.Materials
                .FirstOrDefaultAsync(m=> m.Name.ToLower() == materialName.ToLower());
            if (existingMaterial != null)
            {
                return true;
            }

            return false;
        }
        public async Task<bool> ValidateMainCategory(string mainCategoryName)
        {
            var existingMainCategory = await _db.MainCategories
                .FirstOrDefaultAsync(mc=>mc.Name.ToLower() == mainCategoryName.ToLower());
            if(existingMainCategory != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> ValidateProductColor(string productColorName)
        {
            var existingProductColor = await _db.ProductColors
                .FirstOrDefaultAsync(pc=>pc.Name.ToLower() ==productColorName.ToLower());

            if (existingProductColor != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> ValidateProductSize(string productSizeName)
        {
            var existingProductSize = await _db.ProductSizes
                .FirstOrDefaultAsync(ps=>ps.Name.ToLower() == productSizeName.ToLower());
            if(existingProductSize != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> ValidateMaterialId(int materialId)
        {
            var existingMaterial = await _db.Materials
               .FirstOrDefaultAsync(m => m.Id == materialId);
            if(existingMaterial != null)
            {
                return true;
            }
            return false;

        }

        public async Task<bool> ValidateMainCategoryId(int mainCategoryId)
        {
            var existingMainCategory = await _db.MainCategories
                .FirstOrDefaultAsync(mc=>mc.Id == mainCategoryId);
            if(existingMainCategory != null)
            {
                return true;
            }

            return false;
        }
    }
}
