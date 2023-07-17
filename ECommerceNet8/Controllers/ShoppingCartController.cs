using ECommerceNet8.DTOs.ShoppingCartDtos.Request;
using ECommerceNet8.DTOs.ShoppingCartDtos.Response;
using ECommerceNet8.Repositories.ShoppingCartRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }

        [HttpGet]
        [Route("GetAllShoppingCartItems")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<Response_ShoppingCart>> GetAllShoppingCartItems()
        {
            string userId = HttpContext.User.FindFirstValue("uid");
            var shoppingCartResponse = await _shoppingCartRepository.GetAllCartItems(userId);

            return Ok(shoppingCartResponse);
        }

        [HttpPost]
        [Route("AddCartItem")]
        [Authorize(Roles ="Customer")]
        public async Task<ActionResult<Response_ShoppingCartInfo>> AddCartItem
            ([FromBody]Request_ShoppingCart shoppingCartRequest)
        {
            string userId = HttpContext.User.FindFirstValue("uid");
            var shoppingCartResponse = await _shoppingCartRepository
                .AddItem(userId, shoppingCartRequest);
            if(shoppingCartResponse.IsSuccess == false)
            {
                return BadRequest(shoppingCartResponse);
            }

            return Ok(shoppingCartResponse);
        }

        [HttpPut]
        [Route("UpdateCartItemQty")]
        [Authorize(Roles ="Customer")]
        public async Task<ActionResult<Response_ShoppingCartInfo>> UpdateCartItemQty
            ([FromBody]Request_ShoppingCart shoppingCartRequest)
        {
            string userId = HttpContext.User.FindFirstValue("uid");

            var shoppingCartResponse = await _shoppingCartRepository
                .UpdateQty(userId, shoppingCartRequest);

            if(shoppingCartResponse.IsSuccess == false)
            {
                return BadRequest(shoppingCartResponse);
            }

            return Ok(shoppingCartResponse);
        }

        [HttpDelete]
        [Route("RemoveCartItem")]
        [Authorize(Roles ="Customer")]
        public async Task<ActionResult<Response_ShoppingCartInfo>> RemoveCartItem
            ([FromBody]Request_ShoppingCart shoppingCartRequest)
        {
            string userId = HttpContext.User.FindFirstValue("uid");

            var shoppingCartResponse = await _shoppingCartRepository
                .RemoveItem(userId, shoppingCartRequest);

            if(shoppingCartResponse.IsSuccess  == false)
            {
                return BadRequest(shoppingCartResponse);
            }
            return Ok(shoppingCartResponse);
        }

        [HttpDelete]
        [Route("RemoveAllCartItems")]
        [Authorize(Roles ="Customer")]
        public async Task<ActionResult<Response_ShoppingCartInfo>> RemoveAllCartItems()
        {
            string userId = HttpContext.User.FindFirstValue("uid");

            var shoppingCartResponse = await _shoppingCartRepository.ClearCart(userId);
            if(shoppingCartResponse.IsSuccess == false)
            {
                return BadRequest(shoppingCartResponse);
            }

            return Ok(shoppingCartResponse);
        }
    }
}
