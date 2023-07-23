using ECommerceNet8.DTOs.AddressDtos.Request;
using ECommerceNet8.DTOs.AddressDtos.Response;
using ECommerceNet8.Models.OrderModels;
using ECommerceNet8.Repositories.AddressRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressRepository _addressRepository;

        public AddressController(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        [HttpGet]
        [Route("GetAllAddress")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAllAddresses()
        {
            var addresses = await _addressRepository.GetAllAddresses();
            return Ok(addresses);
        }

        [HttpGet]
        [Route("GetAddressById/{addressId}")]
        [ActionName("GetAddressById")]
        public async Task<ActionResult<Address>> GetAddressById(
            [FromRoute]int addressId)
        {
            var address = await _addressRepository.GetAddressById(addressId);
            if(address == null)
            {
                return BadRequest("No Address Found With Given Id");
            }

            return Ok(address);
        }

        [HttpGet]
        [Route("GetAddressesByUserId/{userId}")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddressesByUserId(
            [FromRoute]string userId)
        {
            var addresses = await _addressRepository.GetAddressesByUserId(userId);
            return Ok(addresses);
        }

        [HttpPost]
        [Route("AddAddress/{userId}")]
        public async Task<IActionResult> AddAddress(
            [FromRoute]string userId, [FromBody]Request_AddressInfo addressInfo)
        {
            var address = await _addressRepository.AddAddress(userId, addressInfo);

            return CreatedAtAction(nameof(GetAddressById),
                new { addressId = address.AddressId }, address);
        }

        [HttpPut]
        [Route("UpdateAddress/{addressId}")]
        public async Task<ActionResult<Response_AddressInfo>> UpdateAddress(
            [FromRoute]int addressId, [FromBody]Request_AddressInfo addressInfo)
        {
            var addressResponse = await _addressRepository.UpdateAddress(addressId, addressInfo);
            if(addressResponse.isSuccess == false)
            {
                return BadRequest(addressResponse);
            }

            return Ok(addressResponse);
        }

        [HttpDelete]
        [Route("DeleteAddress/{addressId}")]
        public async Task<ActionResult<Response_AddressInfo>> DeleteAddress(
            [FromRoute]int addressId)
        {
            var addressResponse = await _addressRepository.DeleteAddress(addressId);
            if(addressResponse.isSuccess == false)
            {
                return BadRequest(addressResponse);
            }
            return Ok(addressResponse);
        }

    }
}
