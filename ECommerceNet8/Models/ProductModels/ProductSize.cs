﻿using System.Text.Json.Serialization;

namespace ECommerceNet8.Models.ProductModels
{
    public class ProductSize
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public IEnumerable<ProductVariant>  productVariants { get; set; }
    }
}
