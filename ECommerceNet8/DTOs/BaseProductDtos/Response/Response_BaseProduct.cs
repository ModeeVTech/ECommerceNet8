using ECommerceNet8.DTOs.BaseProductDtos.CustomModels;

namespace ECommerceNet8.DTOs.BaseProductDtos.Response
{
    public class Response_BaseProduct
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }

        public List<Model_BaseProductWithNoExtraInfo> baseProducts { get; set; }
        = new List<Model_BaseProductWithNoExtraInfo>();
    }
}
