using ECommerceNet8.DTOs.AddressDtos.Request;
using ECommerceNet8.DTOs.AddressDtos.Response;
using ECommerceNet8.Models.OrderModels;

namespace ECommerceNet8.Repositories.AddressRepository
{
    public interface IAddressRepository
    {
        public Task<IEnumerable<Address>> GetAllAddresses();
        public Task<IEnumerable<Address>> GetAddressesByUserId(string userId);
        public Task<Address> GetAddressById(int addresId);
        public Task<Address> AddAddress(string userId, Request_AddressInfo addressInfo);
        public Task<Response_AddressInfo> UpdateAddress
            (int addressId, Request_AddressInfo addressInfo);
        public Task<Response_AddressInfo> DeleteAddress(int addressId);
    }
}
