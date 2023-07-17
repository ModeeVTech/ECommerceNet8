using ECommerceNet8.DTOs.ShoppingCartDtos.Models;

namespace ECommerceNet8.DTOs.ShoppingCartDtos.Response
{
    public class Response_ShoppingCart
    {
        public int ShoppingCartId { get; set; }
        public bool CanBeSold { get; set; }
        public string Message { get; set; }
        public decimal TotalPrice { get; set; }
        public List<Model_CartItemReturn> CartItemsCanBeSold { get; set; } = new List<Model_CartItemReturn>();
        public List<Model_CartItemReturn> CartItemsCantBeSold { get; set; } = new List<Model_CartItemReturn>();
    }
}
