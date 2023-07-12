using ECommerceNet8.DTOs.ProducSizeDtos.Request;
using ECommerceNet8.DTOs.ProducSizeDtos.Response;
using ECommerceNet8.DTOs.ProductVariantDtos.Response;
using ECommerceNet8.Models.ProductModels;
using ECommerceNet8.Repositories.ProductSizeRepository;
using ECommerceNet8.Repositories.ValidationsRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSizeController : ControllerBase
    {
        private readonly IProductSizeRepository _productSizeRepository;
        private readonly IValidationRepository _validationRepository;

        public ProductSizeController(IProductSizeRepository productSizeRepository,
            IValidationRepository validationRepository)
        {
            _productSizeRepository = productSizeRepository;
            _validationRepository = validationRepository;
        }


        [HttpGet]
        [Route("GetAllProductSizes")]
        public async Task<ActionResult<Response_ProductSize>> GetAllProductSizes()
        {
            var productSizeResponse = await _productSizeRepository.GetAllProductSizes();
            if(productSizeResponse.isSuccess == false)
            {
                return BadRequest(productSizeResponse);
            }

            return Ok(productSizeResponse);
        }

        [HttpGet]
        [Route("GetProductSizeById/{productSizeId}")]
        [ActionName("GetProductSizeById")]
        public async Task<ActionResult<Response_ProductSize>> GetProductSizeById(
            [FromRoute]int productSizeId)
        {
            var productSizeResponse = await _productSizeRepository
                .GetProductSizeById(productSizeId);
            if(productSizeResponse.isSuccess == false)
            {
                return NotFound(productSizeResponse);
            }

            return Ok(productSizeResponse);
        }


        [HttpPost]
        [Route("AddProductSize")]
        public async Task<IActionResult> AddProductSize(
            [FromBody]Request_ProductSize productSize)
        {
            var checkResponse = await CheckIfSizeExist(productSize.Name);
            if(checkResponse == true)
            {
                return BadRequest("Size already exist");
            }

            var productSizeResponse = await _productSizeRepository.AddProductSize(productSize);

            return CreatedAtAction(nameof(GetProductSizeById),
                new { productSizeId = productSizeResponse.ProductSizes[0].Id },
                productSizeResponse.ProductSizes[0]);
        }

        [HttpPut]
        [Route("UpdateProductSize/{productSizeId}")]
        public async Task<ActionResult<Response_ProductSize>> UpdateProductSize(
            [FromRoute]int productSizeId, [FromBody] Request_ProductSize productSize)
        {
            var checkResponse = await CheckIfSizeExist(productSize.Name);
            if(checkResponse == true)
            {
                return BadRequest("Size already exist");
            }

            var produtSizeResponse = await _productSizeRepository
                .UpdateProductSize(productSizeId, productSize);
            if(produtSizeResponse.isSuccess == false)
            {
                return NotFound(produtSizeResponse);
            }
            return Ok(produtSizeResponse);
        }

        [HttpDelete]
        [Route("DeleteProductSize/{productSizeId}")]
        public async Task<ActionResult<Response_ProductSize>> DeleteProductSize(
            [FromRoute]int productSizeId)
        {
            var productSizeResponse = await _productSizeRepository
                .DeleteProductSize(productSizeId);

            if(productSizeResponse.isSuccess == false)
            {
                return NotFound(productSizeResponse);
            }

            return Ok(productSizeResponse);
        }


        #region Validations
        private async Task<bool> CheckIfSizeExist(string productSizeName)
        {
            var response = await _validationRepository
                .ValidateProductSize(productSizeName);

            return response;
        }
        #endregion
    }
}
