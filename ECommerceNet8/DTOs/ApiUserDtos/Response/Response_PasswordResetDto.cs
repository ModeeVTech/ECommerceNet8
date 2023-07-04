namespace ECommerceNet8.DTOs.ApiUserDtos.Response
{
    public class Response_PasswordResetDto
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }  = new List<string>();
    }
}
