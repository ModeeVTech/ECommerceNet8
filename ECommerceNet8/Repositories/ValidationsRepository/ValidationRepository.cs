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
    }
}
