using System.Text.Json.Serialization;

namespace ECommerceNet8.Models.ShoppingCartModels
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public int ShoppingCartId { get; set; }
        [JsonIgnore]
        public ShoppingCart shoppingCart { get; set; }
        public int Quantity { get; set; }
    }
}
