using ECommerceNet8.DTOs.OrderDtos.Request;
using ECommerceNet8.DTOs.OrderDtos.Response;
using ECommerceNet8.Models.OrderModels;
using ECommerceNet8.Repositories.OrderRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ECommerceNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }


        [HttpGet]
        [Route("GetAllOrders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrders();

            return Ok(orders);
        }

        [HttpGet]
        [Route("GetNotSentOrders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetNotSentOrders()
        {
            var orders = await _orderRepository.GetNotSentOrders();

            return Ok(orders);
        }

        [HttpPost]
        [Route("MarkOrderAsSent/{orderId}")]
        public async Task<ActionResult<Response_Order>> MarkOrderAsSent(
            [FromRoute]int orderId)
        {
            var orderResponse = await _orderRepository.MarkOrderAsSent(orderId);
            if(orderResponse.isSuccess == false)
            {
                return BadRequest(orderResponse);
            }

            return Ok(orderResponse);
        }

        [HttpPost]
        [Route("MarkOrderAsNotSent/{orderId}")]
        public async Task<ActionResult<Response_Order>> MarkOrderAsNotSent(
            [FromRoute]int orderId)
        {
            var orderResponse = await _orderRepository.MarkOrderAsNotSent(orderId);
            if(orderResponse.isSuccess == false)
            {
                return BadRequest(orderResponse);
            }
            return Ok(orderResponse);
        }

        [HttpGet]
        [Route("GetOrder/{uniqueOrderIdentifier}")]
        public async Task<ActionResult<Order>> GetOrder(
            [FromRoute]string uniqueOrderIdentifier)
        {
            var order = await _orderRepository.GetOrder(uniqueOrderIdentifier);
            if(order == null)
            {
                return NotFound("No order found with given Id");
            }

            return Ok(order);
        }

        [HttpPost]
        [Route("GetOrdersByDate")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByDate(
            [FromBody]Request_OrderDate orderDate)
        {
            var orders = await _orderRepository.GetAllOrderByDate(orderDate);


            if(orders.Count() <= 0)
            {
                return BadRequest("No Orders Found");
            }

            return Ok(orders);
        }

        [HttpGet]
        [Route("GetItemsAtCustomer/{orderUniqueIdentifier}")]
        public async Task<ActionResult<Response_ItemsAtCustomer>> GetItemsAtCustomer(
            [FromRoute] string orderUniqueIdentifier)
        {
            var itemsAtCustomerResponse = await _orderRepository
                .GetItemsAtCustomer(orderUniqueIdentifier);
            if(itemsAtCustomerResponse.isSuccess == false)
            {
                return NotFound(itemsAtCustomerResponse);
            }

            return Ok(itemsAtCustomerResponse);
        }

        [HttpGet]
        [Route("GetPdf/{OrderUniqueIdentifier}")]
        public async Task<IActionResult> GetPdf([FromRoute]string OrderUniqueIdentifier)
        {
            var order = await _orderRepository.GetOrderForPdf(OrderUniqueIdentifier);
            if(order == null)
            {
                return NotFound("Check Unique Id");
            }

            var date = order.OrderTime.ToString();
            var dateNormalized = date.Replace("/", "_");
            string fileName = "PDF_" + dateNormalized + ".pdf";

            var provider = new FileExtensionContentTypeProvider();
            string filePath = order.OriginalOrderFromCustomer.pdfInfo.Path;
            string contentType = "application/octet-stream";
            byte[] fileBytes;

            fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, contentType, fileName);

        }

        [HttpPost]
        [Route("CreateOrder/{userId}/{userAddressId}/{shippingTypeId}")]
        public async Task<ActionResult<Response_Order>> CreateOrder(
            [FromRoute]string userId, [FromRoute]int userAddressId,
            [FromRoute]int shippingTypeId)
        {
            var orderResponse = await _orderRepository
                .GenerateOrder(userId, userAddressId, shippingTypeId);
            if(orderResponse.isSuccess  == false)
            {
                return BadRequest(orderResponse);
            }

            return Ok(orderResponse);
        }
    }
}
