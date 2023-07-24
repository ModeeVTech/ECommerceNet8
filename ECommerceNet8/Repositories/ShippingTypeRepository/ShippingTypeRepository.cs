using ECommerceNet8.Data;
using ECommerceNet8.DTOs.ShippingTypeDtos.Request;
using ECommerceNet8.DTOs.ShippingTypeDtos.Response;
using ECommerceNet8.Models.OrderModels;
using Microsoft.EntityFrameworkCore;

namespace ECommerceNet8.Repositories.ShippingTypeRepository
{
    public class ShippingTypeRepository : IShippingTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public ShippingTypeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ShippingType>> GetShippingTypes()
        {
            var shippingTypes = await _db.ShippingTypes.ToListAsync();
            return shippingTypes;
        }
        public async Task<ShippingType> GetShippingTypeById(int shippingTypeId)
        {
            var shippingType  = await _db.ShippingTypes
                .FirstOrDefaultAsync(st=>st.ShippingTypeId  == shippingTypeId);
            return shippingType;
        }

        public async Task<ShippingType> AddShippingType(Request_ShippingType shippingType)
        {
            ShippingType shippingTypeBase = new ShippingType()
            {
                ShippingFirmName = shippingType.ShippingFirmName,
                Price = shippingType.Price,
                FreeTier = shippingType.FreeTier,
            };

            await _db.ShippingTypes.AddAsync(shippingTypeBase);
            await _db.SaveChangesAsync();

            return shippingTypeBase;
        }

        public async Task<Response_ShippingType> UpdateShippingType(int shippingTypeId, Request_ShippingType shippingType)
        {
            var existingShippingType = await _db.ShippingTypes
                .FirstOrDefaultAsync(st => st.ShippingTypeId == shippingTypeId);

            if (existingShippingType == null)
            {
                return new Response_ShippingType()
                {
                    isSuccess = false,
                    Message = "No shipping type found with given Id"
                };
            }

            existingShippingType.ShippingFirmName = shippingType.ShippingFirmName;
            existingShippingType.Price = shippingType.Price;
            existingShippingType.FreeTier  = shippingType.FreeTier;

            await _db.SaveChangesAsync();

            return new Response_ShippingType()
            {
                isSuccess = true,
                Message = "Shipping type updated"
            };
        }

        public async Task<Response_ShippingType> DeleteShippingType(int shippingTypeId)
        {
            var existingShippingType = await _db.ShippingTypes
                .FirstOrDefaultAsync(st => st.ShippingTypeId == shippingTypeId);

            if(existingShippingType == null)
            {
                return new Response_ShippingType()
                {
                    isSuccess = false,
                    Message = "No Shipping Type Found With Given Id"
                };
            }

            _db.ShippingTypes.Remove(existingShippingType);
            await _db.SaveChangesAsync();

            return new Response_ShippingType()
            {
                isSuccess = true,
                Message = "Shipping Type Removed"
            };
        }

    }
}
