namespace ECommerceNet8.DTOs.ProductVariantDtos.Response
{
    public class Response_ProductVariantCheckQty
    {
        public int productVariantId { get; set; }
        public bool productExist { get; set; }
        public int requestQty { get; set; }
        public int hasQty { get; set; }
        public bool CanBeSold { get; set; }
    }
}
