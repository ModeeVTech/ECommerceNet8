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
    }
}
