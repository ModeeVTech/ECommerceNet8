using System.ComponentModel.DataAnnotations;

namespace ECommerceNet8.DTOs.ApiUserDtos.Request
{
    public class Request_LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
