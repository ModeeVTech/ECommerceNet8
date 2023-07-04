using System.ComponentModel.DataAnnotations;

namespace ECommerceNet8.DTOs.ApiUserDtos.Request
{
    public class Request_PasswordResetDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength =8)]
        public string newPassword { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 8)]
        public string confirmPassword { get; set; }
    }
}
