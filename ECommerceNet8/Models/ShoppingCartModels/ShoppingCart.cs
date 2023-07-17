using ECommerceNet8.Models.AuthModels;
using System.Text.Json.Serialization;

namespace ECommerceNet8.Models.ShoppingCartModels
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public string ApiUserId { get; set; }
        [JsonIgnore]
        public ApiUser ApiUser { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
