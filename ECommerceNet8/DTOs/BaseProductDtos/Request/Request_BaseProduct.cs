namespace ECommerceNet8.DTOs.BaseProductDtos.Request
{
    public class Request_BaseProduct
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int MainCategoryId { get; set; }
        public int MaterialId { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
    }
}
