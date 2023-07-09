using ECommerceNet8.DTOs.BaseProductDtos.CustomModels;

namespace ECommerceNet8.DTOs.BaseProductDtos.Response
{
    public class Response_BaseProductWithPaging
    {
        public List<Model_BaseProductCustom> baseProducts { get; set; }
        = new List<Model_BaseProductCustom>();

        public int TotalPages { get; set; }
    }
}
