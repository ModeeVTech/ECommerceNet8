namespace ECommerceNet8.DTOs.ApiUserDtos.Response
{
    public class Response_LoginDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Result { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
