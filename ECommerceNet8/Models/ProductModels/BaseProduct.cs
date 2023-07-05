using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceNet8.Models.ProductModels
{
    public class BaseProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MainCategoryId { get; set; }
        public MainCategory MainCategory { get; set; }
        public int MaterialId { get; set; }
        public Material Material { get; set; }

        public ICollection<ProductVariant> productVariants { get; set; }
        public ICollection<ImageBase> imageBases { get; set; }

        [Column(TypeName ="decimal(18,2)")]
        public decimal Price { get; set; }
        public int Discount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

    }
}
