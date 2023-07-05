using ECommerceNet8.DTOs.MainCategoryDtos.Request;
using ECommerceNet8.DTOs.MainCategoryDtos.Response;
using ECommerceNet8.Models.ProductModels;
using ECommerceNet8.Repositories.MainCategoryRepository;
using ECommerceNet8.Repositories.ValidationsRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainCategoryController : ControllerBase
    {
        private readonly IMainCategoryRepository _mainCategoryRepository;
        private readonly IValidationRepository _validationRepository;

        public MainCategoryController(IMainCategoryRepository mainCategoryRepository,
            IValidationRepository validationRepository)
        {
            _mainCategoryRepository = mainCategoryRepository;
            _validationRepository = validationRepository;
        }

        [HttpGet]
        [Route("GetAllMainCategories")]
        public async Task<ActionResult<Response_MainCategory>> GetAllMainCategories()
        {
            var mainCategoryResponse = await _mainCategoryRepository
                .GetAllMainCategories();
            if(mainCategoryResponse.isSuccess == false)
            {
                return NotFound(mainCategoryResponse);
            }
            return Ok(mainCategoryResponse);
        }
        [HttpGet]
        [Route("GetMainCategory/{mainCategoryId}")]
        [ActionName(nameof(GetMainCategoryById))]
        public async Task<ActionResult<Response_MainCategory>> GetMainCategoryById(
            [FromRoute]int mainCategoryId)
        {
            var mainCategoryResponse = await _mainCategoryRepository
                .GetMainCategoryById(mainCategoryId);

            if(mainCategoryResponse.isSuccess == false)
            {
                return NotFound(mainCategoryResponse);
            }
            return Ok(mainCategoryResponse);
        }

        [HttpPost]
        [Route("AddMainCategory")]
        public async Task<IActionResult> AddMainCategory(
            [FromBody]Request_MainCategory mainCategory)
        {
            var checkResponse = await MainCategoryCheck(mainCategory);
            if(checkResponse ==  true)
            {
                return BadRequest("Category Already Exist");
            }
            var mainCategoryResponse = await _mainCategoryRepository
                .AddMainCategory(mainCategory);

            return CreatedAtAction(nameof(GetMainCategoryById),
                new { mainCategoryId = mainCategoryResponse.mainCategories[0].Id },
                mainCategoryResponse.mainCategories[0]);
        }
        [HttpPut]
        [Route("UpdateMainCategory/{mainCategoryId}")]
        public async Task<ActionResult<Response_MainCategory>>  UpdateMainCategory
            ([FromRoute]int mainCategoryId, [FromBody]  Request_MainCategory mainCategory)
        {
            var checkResponse = await MainCategoryCheck(mainCategory);
            if(checkResponse == true)
            {
                return BadRequest("Main Category Already Exist");
            }

            var mainCategoryResponse = await _mainCategoryRepository
                .UpdateMainCategory(mainCategoryId, mainCategory);

            if(mainCategoryResponse.isSuccess == false)
            {
                return NotFound(mainCategoryResponse);
            }

            return Ok(mainCategoryResponse);

        }

        [HttpDelete]
        [Route("DeleteMainCategory/{mainCategoryId}")]
        public async Task<ActionResult<Response_MainCategory>> DeleteMainCategory
            ([FromRoute]int mainCategoryId)
        {
            var mainCategoryResponse = await _mainCategoryRepository
                .DeleteMainCategory(mainCategoryId);
            if(mainCategoryResponse.isSuccess == false)
            {
                return BadRequest(mainCategoryResponse);
            }

            return Ok(mainCategoryResponse);
        }

        #region Validations
        private async Task<bool> MainCategoryCheck(Request_MainCategory mainCategory)
        {
            var response = await _validationRepository
                .ValidateMainCategory(mainCategory.Name);
            return response;

        }
        #endregion
    }
}
