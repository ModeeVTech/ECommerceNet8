using ECommerceNet8.DTOs.OrderDtos.Request;
using ECommerceNet8.DTOs.OrderDtos.Response;
using ECommerceNet8.Models.OrderModels;

namespace ECommerceNet8.Repositories.OrderRepository
{
    public interface IOrderRepository
    {
        public Task<Response_Order> GenerateOrder
            (string userId, int userAddressId, int shippingTypeId);

        public Task<Order> GetOrder(string OrderUniqueIdentifier);
        public Task<Order> GetOrderForPdf(string OrderUniqueIdentifier);
        public Task<IEnumerable<Order>> GetAllOrders();
        public Task<IEnumerable<Order>> GetNotSentOrders();
        public Task<Response_Order> MarkOrderAsSent(int orderId);
        public Task<Response_Order> MarkOrderAsNotSent(int orderId);
        public Task<Response_ItemsAtCustomer> GetItemsAtCustomer(string OrderUniqueIdentifier);

        public Task<IEnumerable<Order>> GetAllOrderByDate(Request_OrderDate orderDate);

    }
}
