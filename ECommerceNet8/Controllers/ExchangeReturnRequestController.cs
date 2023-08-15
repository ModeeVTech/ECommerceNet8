using ECommerceNet8.Constants;
using ECommerceNet8.DTOs.RequestExchangeDtos.Request;
using ECommerceNet8.DTOs.RequestExchangeDtos.Response;
using ECommerceNet8.DTOs.ReturnDtos.Request;
using ECommerceNet8.DTOs.ReturnDtos.Response;
using ECommerceNet8.Models.ReturnExchangeModels;
using ECommerceNet8.Repositories.ReturnExchangeRequestRepository;
using ECommerceNet8.Repositories.ReturnRequestRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeReturnRequestController : ControllerBase
    {
        private readonly IReturnExchangeRequestRepository _returnExchangeRequestRepository;
        private readonly IReturnRequestRepository _returnRequestRepository;

        public ExchangeReturnRequestController(IReturnExchangeRequestRepository returnExchangeRequestRepository,
            IReturnRequestRepository returnRequestRepository)
        {
            _returnExchangeRequestRepository = returnExchangeRequestRepository;
            _returnRequestRepository = returnRequestRepository;
        }

        [HttpGet]
        [Route("GetAllExchangeRequests")]
        public async Task<ActionResult<ICollection<ExchangeRequestFromUser>>>
            GetAllExchangeRequests()
        {
            var response = await _returnExchangeRequestRepository.GetExchangeRequestFromUsers();

            return Ok(response);
        }
        [HttpGet]
        [Route("ExchangeRequestByOrderUniqueId/{OrderUniqueIdentifier}")]
        public async Task<ActionResult<ICollection<ExchangeRequestFromUser>>>
            ExchangeRequestByOrderUniqueId([FromRoute]string OrderUniqueIdentifier)
        {
            var response = await _returnExchangeRequestRepository
                .GetExchangeRequestByOrderUniqueIdentifier(OrderUniqueIdentifier);

            return Ok(response);
        }
        [HttpGet]
        [Route("ExchangeRequestByExchangeUniqueId/{ExchangeUniqueIdentifier}")]
        public async Task<ActionResult<ExchangeRequestFromUser>> ExchangeRequestByExchangeUniqueId
            ([FromRoute]string ExchangeUniqueIdentifier)
        {
            var response = await _returnExchangeRequestRepository
                .GetExchangeRequestByExchangeUniqueId(ExchangeUniqueIdentifier);

            if(response == null)
            {
                return NotFound("No Exchange Request");
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("AddExchangeRequest")]
        [Authorize(Roles = Roles.Customer)]
        public async Task<ActionResult<Response_ExchangeRequest>> AddExchangeRequest
            ([FromBody]Request_ExchangeRequest exchangeRequest)
        {
            var userId = HttpContext.User.FindFirstValue("uid");
            var response = await _returnExchangeRequestRepository.AddExchangeRequest
                (exchangeRequest, userId);
            if(response.isSuccess == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        [HttpGet]
        [Route("GetAllReturnRequest")]
        public async Task<ActionResult<ICollection<ReturnRequestFromUser>>> GetAllReturnRequest()
        {
            var response = await _returnRequestRepository.GetAllReturnRequest();

            return Ok(response);
        }

        [HttpGet]
        [Route("ReturnRequestByOUI/{OrderUniqueIdentifier}")]
        public async Task<ActionResult<ICollection<ReturnRequestFromUser>>>
            ReturnRequestByOUI([FromRoute]string OrderUniqueIdentifier)
        {
            var response = await _returnRequestRepository
                .GetReturnRequestByOrderUniqueId(OrderUniqueIdentifier);

            return Ok(response);
        }

        [HttpGet]
        [Route("ReturnRequestByEUI/{ExchangeUniqueIdentifier}")]
        public async Task<ActionResult<ReturnRequestFromUser>>
            ReturnRequestByEUI([FromRoute]string ExchangeUniqueIdentifier)
        {
            var response = await _returnRequestRepository
                .GetReturnRequestByExchangeUniqueId(ExchangeUniqueIdentifier);
            if(response == null)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("AddReturnRequest")]
        [Authorize(Roles =Roles.Customer)]
        public async Task<ActionResult<Response_ReturnRequest>> AddReturnRequest
            ([FromBody]Request_ReturnRequest returnRequest)
        {
            var userId = HttpContext.User.FindFirstValue("uid");
            var response = await _returnRequestRepository
                .AddReturnRequest(returnRequest, userId);

            if(response.isSuccess == false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
