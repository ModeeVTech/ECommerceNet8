using System.ComponentModel.DataAnnotations;

namespace ECommerceNet8.DTOs.ApiUserDtos.Request
{
    public class Request_ApiUserRegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(15, 
            ErrorMessage ="Your password is limited to 8 and 15 characters",
            MinimumLength =8)]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}
