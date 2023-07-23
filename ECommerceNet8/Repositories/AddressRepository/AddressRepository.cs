using ECommerceNet8.Data;
using ECommerceNet8.DTOs.AddressDtos.Request;
using ECommerceNet8.DTOs.AddressDtos.Response;
using ECommerceNet8.Models.OrderModels;
using Microsoft.EntityFrameworkCore;

namespace ECommerceNet8.Repositories.AddressRepository
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ApplicationDbContext _db;

        public AddressRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Address>> GetAllAddresses()
        {
            var addresses = await _db.Addresses.ToListAsync();
            return addresses;
        }
        public async Task<Address> GetAddressById(int addressId)
        {
            var address = await _db.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == addressId);

            return address;
        }
        public async Task<IEnumerable<Address>> GetAddressesByUserId(string userId)
        {
            var addresses = await _db.Addresses
                .Where(a => a.UserId == userId).ToListAsync();
            return addresses;
        }
        public async Task<Address> AddAddress(string userId, Request_AddressInfo addressInfo)
        {
            Address address = new Address()
            {
                UserId = userId,
                Street = addressInfo.Street,
                HouseNumber = addressInfo.HouseNumber,
                AppartmentNumber = addressInfo.ApparmentNumber,
                City = addressInfo.City,
                PostalCode = addressInfo.PostalCode,
                Country = addressInfo.Country,
                Region = addressInfo.Region,
            };

            await _db.Addresses.AddAsync(address);
            await _db.SaveChangesAsync();

            return address;
        }
        public async Task<Response_AddressInfo> UpdateAddress(int addressId, Request_AddressInfo addressInfo)
        {
            var existingAddress = await _db.Addresses
                .FirstOrDefaultAsync(a=>a.AddressId == addressId);

            if(existingAddress == null)
            {
                return new Response_AddressInfo()
                {
                    isSuccess = false,
                    Message = "No Addresses found with given Address Id"
                };
            }

            existingAddress.Street = addressInfo.Street;
            existingAddress.HouseNumber = addressInfo.HouseNumber;
            existingAddress.AppartmentNumber = addressInfo.ApparmentNumber;
            existingAddress.City = addressInfo.City;
            existingAddress.Region = addressInfo.Region;
            existingAddress.Country = addressInfo.Country;
            existingAddress.PostalCode = addressInfo.PostalCode;

            await _db.SaveChangesAsync();
            return new Response_AddressInfo()
            {
                isSuccess = true,
                Message = "Address updated successfully"
            };
        }

        public async Task<Response_AddressInfo> DeleteAddress(int addressId)
        {
            var existingAddress = await _db.Addresses
                .FirstOrDefaultAsync(a=>a.AddressId ==addressId);
            if(existingAddress == null)
            {
                return new Response_AddressInfo()
                {
                    isSuccess = false,
                    Message = "No Addresses Found With Given Address Id"
                };
            }

            _db.Addresses.Remove(existingAddress);
            await _db.SaveChangesAsync();

            return new Response_AddressInfo()
            {
                isSuccess = true,
                Message = "Address Deleted Successfully"
            };
        }
    }
}
