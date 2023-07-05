using ECommerceNet8.DTOs.ProductColorDtos.Request;
using ECommerceNet8.DTOs.ProductColorDtos.Response;
using ECommerceNet8.Models.ProductModels;
using ECommerceNet8.Repositories.ProductColorRepository;
using ECommerceNet8.Repositories.ValidationsRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductColorController : ControllerBase
    {
        private readonly IProductColorRepository _productColorRepository;
        private readonly IValidationRepository _validationRepository;

        public ProductColorController(IProductColorRepository productColorRepository,
            IValidationRepository validationRepository)
        {
            _productColorRepository = productColorRepository;
            _validationRepository = validationRepository;
        }

        [HttpGet]
        [Route("GetAllProductColors")]
        public async Task<ActionResult<Response_ProductColor>> GetAllProductColors()
        {
            var response = await _productColorRepository.GetAllProductColors();
            if(response.isSuccess == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [HttpGet]
        [Route("GetProductColorById/{productColorId}")]
        [ActionName("GetProductColorById")]
        public async Task<ActionResult<Response_ProductColor>> GetProductColorById(
            [FromRoute]int productColorId)
        {
            var response = await _productColorRepository.GetProductColorById(productColorId);
            if(response.isSuccess == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("AddProductColor")]
        public async Task<IActionResult> AddProductColor(
            [FromBody]Request_ProductColor productColor)
        {
            var ColorResponse = await CheckIfColorExist(productColor.Name);
            if(ColorResponse == true)
            {
                return BadRequest("Color Already Exist");
            }

            var productColorResponse = await _productColorRepository
                .AddProductColor(productColor);

            return CreatedAtAction(nameof(GetProductColorById),
                new { productColorId = productColorResponse.productColors[0].Id },
                productColorResponse.productColors[0]);
        }

        [HttpPut]
        [Route("UpdateProductColor/{productColorId}")]
        public async Task<ActionResult<Response_ProductColor>> UpdateProductColor(
            [FromRoute]int productColorId, [FromBody]Request_ProductColor productColor)
        {
            var ColorResponse = await CheckIfColorExist(productColor.Name);
            if(ColorResponse == true)
            {
                return BadRequest("Product Color Already Exist");
            }

            var productColorResponse = await _productColorRepository
                .UpdateProductColor(productColorId, productColor);
            if(productColorResponse.isSuccess == false)
            {
                return NotFound(productColorResponse);
            }

            return Ok(productColorResponse);
        }

        [HttpDelete]
        [Route("RemoveProductColor/{productColorId}")]
        public async Task<ActionResult<Response_ProductColor>> RemoveProductColor(
            [FromRoute]int productColorId)
        {
            var response  = await _productColorRepository
                .DeleteProductColor(productColorId);

            if(response.isSuccess  == false)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        #region Validations
        private async Task<bool> CheckIfColorExist(string productColorName)
        {
            var response = await _validationRepository.ValidateProductColor(productColorName);

            return response;
        }
        #endregion
    }
}
