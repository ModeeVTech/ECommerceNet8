using Microsoft.AspNetCore.Identity;

namespace ECommerceNet8.Models.AuthModels
{
    public class ApiUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
