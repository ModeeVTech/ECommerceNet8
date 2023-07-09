using ECommerceNet8.DTOs.BaseProductDtos.CustomModels;

namespace ECommerceNet8.DTOs.BaseProductDtos.Response
{
    public class Response_BaseProductWithFullInfo
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public Model_BaseProductCustom baseProduct { get; set; }
    }
}
