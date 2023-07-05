using System.Text.Json.Serialization;

namespace ECommerceNet8.Models.ProductModels
{
    public class MainCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public IEnumerable<BaseProduct> baseProducts { get; set; }
    }
}
