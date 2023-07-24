using ECommerceNet8.DTOs.ShippingTypeDtos.Request;
using ECommerceNet8.DTOs.ShippingTypeDtos.Response;
using ECommerceNet8.Models.OrderModels;
using ECommerceNet8.Repositories.ShippingTypeRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingTypeController : ControllerBase
    {
        private readonly IShippingTypeRepository _shippingTypeRepository;

        public ShippingTypeController(IShippingTypeRepository shippingTypeRepository)
        {
            _shippingTypeRepository = shippingTypeRepository;
        }

        [HttpGet]
        [Route("GetAllShippingTypes")]
        public async Task<ActionResult<IEnumerable<ShippingType>>> GetAllShippingTypes()
        {
            var shippingTypes = await  _shippingTypeRepository.GetShippingTypes();
            return  Ok(shippingTypes);
        }

        [HttpGet]
        [Route("GetShippingTypeById/{shippingTypeId}")]
        [ActionName("GetShippingTypeById")]
        public async Task<ActionResult<ShippingType>> GetShippingTypeById
            ([FromRoute] int shippingTypeId)
        {
            var shippingType = await _shippingTypeRepository
                .GetShippingTypeById(shippingTypeId);

            if(shippingType == null)
            {
                return NotFound("No Shipping Type Found");
            }

            return Ok(shippingType);
        }

        [HttpPost]
        [Route("AddShippingType")]
        public async Task<IActionResult> AddShippingType
            ([FromBody]Request_ShippingType shippingType)
        {
            var shippingTypeResponse = await _shippingTypeRepository
                .AddShippingType(shippingType);

            return CreatedAtAction(nameof(GetShippingTypeById),
                new { shippingTypeId = shippingTypeResponse.ShippingTypeId }, 
                shippingTypeResponse);
        }

        [HttpPut]
        [Route("UpdateShippingType/{shippingTypeId}")]
        public async Task<ActionResult<Response_ShippingType>> UpdateShippingType
            ([FromRoute]int shippingTypeId, [FromBody]Request_ShippingType shippingType)
        {
            var shippingTypeResponse = await _shippingTypeRepository
                .UpdateShippingType(shippingTypeId, shippingType);

            if(shippingTypeResponse.isSuccess == false)
            {
                return BadRequest(shippingTypeResponse);
            }

            return Ok(shippingTypeResponse);
        }

        [HttpDelete]
        [Route("DeleteShippingType/{shippingTypeId}")]
        public async Task<ActionResult<Response_ShippingType>> DeleteShippingType
            ([FromRoute]int shippingTypeId)
        {
            var shippingTypeResponse = await _shippingTypeRepository
                .DeleteShippingType(shippingTypeId);

            if(shippingTypeResponse.isSuccess == false)
            {
                return NotFound(shippingTypeResponse);
            }

            return Ok(shippingTypeResponse);
        }
    }
}
