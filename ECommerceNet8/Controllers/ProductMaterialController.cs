using ECommerceNet8.DTOs.ProductDtos.Request;
using ECommerceNet8.DTOs.ProductDtos.Response;
using ECommerceNet8.Repositories.MaterialRepository;
using ECommerceNet8.Repositories.ValidationsRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductMaterialController : ControllerBase
    {
        private readonly IMaterialRepository _materialRepository;
        private readonly IValidationRepository _validationRepository;

        public ProductMaterialController(IMaterialRepository  materialRepository,
            IValidationRepository validationRepository)
        {
            _materialRepository = materialRepository;
            _validationRepository = validationRepository;
        }

        [HttpGet]
        [Route("GetAllMaterials")]
        public async Task<ActionResult<Response_ProductMaterial>> GetAllMaterials()
        {
            var productMaterialResponse  = await  _materialRepository.GetAllProductMaterials();

            if(productMaterialResponse.isSuccess == false)
            {
                return BadRequest(productMaterialResponse);
            }

            return Ok(productMaterialResponse);
        }

        [HttpGet]
        [Route("GetMaterialById/{materialId}")]
        public async Task<ActionResult<Response_ProductMaterial>> GetMaterialById(
            [FromRoute]int materialId)
        {
            var productMaterialResponse = await _materialRepository
                .GetProductMaterialById(materialId);
            if(productMaterialResponse.isSuccess == false)
            {
                return BadRequest(productMaterialResponse);
            }
            return Ok(productMaterialResponse);
        }

        [HttpPost]
        [Route("AddMaterial")]
        public async Task<IActionResult> AddMaterial
            ([FromBody]  Request_ProductMaterial material)
        {
            //VALIDATION FOR DUBLICATES
            var response = await  CheckMaterial(material);
            if(response == true)
            {
                return BadRequest("Material Already Exist");
            }

            var addMaterialResponse = await _materialRepository
                .AddProductMaterial(material);

            return CreatedAtAction(nameof(GetMaterialById), new { materialId = addMaterialResponse.materials[0].Id }, addMaterialResponse.materials[0]);

        }

        [HttpPut]
        [Route("UpdateMaterial/{materialId}")]
        public async Task<ActionResult<Response_ProductMaterial>> UpdateMaterial
            ([FromRoute] int materialId, [FromBody]Request_ProductMaterial productMaterial)
        {
            //VALIDATION FOR DUBLICATES
            var response = await CheckMaterial(productMaterial);
            if(response == true)
            {
                return BadRequest("Material Already Exist");
            }

            //UPDATE MATERIAL
            var materialResponse = await _materialRepository
                .UpdateProductMaterial(materialId, productMaterial);
            if(materialResponse.isSuccess == false)
            {
                return BadRequest(materialResponse);
            }
            return Ok(materialResponse);
        }

        [HttpDelete]
        [Route("DeleteMaterial/{materialId}")]
        public async Task<ActionResult<Response_ProductMaterial>> DeleteMaterial(
            [FromRoute]int materialId)
        {
            var materialResponse  = await  _materialRepository
                .DeleteProductMaterial(materialId);
            if(materialResponse.isSuccess == false)
            {
                return NotFound(materialResponse);
            }

            return Ok(materialResponse);
        }


        #region Validations
        private async Task<bool> CheckMaterial(Request_ProductMaterial productMaterial)
        {
            //TRUE IF EXIST

            var response = await _validationRepository.ValidateMaterial(productMaterial.Name);

            return response;
        }
        #endregion
    }
}
