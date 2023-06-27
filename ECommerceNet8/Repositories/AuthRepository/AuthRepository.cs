using ECommerceNet8.Constants;
using ECommerceNet8.Data;
using ECommerceNet8.DTOs.ApiUserDtos.Request;
using ECommerceNet8.DTOs.ApiUserDtos.Response;
using ECommerceNet8.Models.AuthModels;
using Microsoft.AspNetCore.Identity;

namespace ECommerceNet8.Repositories.AuthRepository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;

        public AuthRepository(UserManager<ApiUser> userManager,
            IConfiguration configuration,
            ApplicationDbContext  dbContext)
        {
            _userManager = userManager;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public async Task<Response_ApiUserRegisterDto> Register(Request_ApiUserRegisterDto userDto)
        {
            var user = new ApiUser()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
            };

            user.UserName = userDto.Email;
            user.EmailConfirmed = false;

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.Customer);

                return new Response_ApiUserRegisterDto()
                {
                    isSuccess = true,
                    apiUser = user,
                };
            }

            List<string> errors = new List<string>();

            foreach(var error in result.Errors)
            {
                errors.Add(error.Description.ToString());
            }

            return new Response_ApiUserRegisterDto()
            {
                isSuccess = false,
                Message = errors
            };
        }

        public async Task<Response_ApiUserRegisterDto> RegisterAdmin(Request_ApiUserRegisterDto userDto, int secretKey)
        {

            if(secretKey != 12345)
            {
                return new Response_ApiUserRegisterDto()
                {
                    isSuccess = false,
                    Message = new List<string>()
                    {
                        "Wrong secret key"
                    }
                };
            }


            var user = new ApiUser()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
            };

            user.UserName = userDto.Email;
            user.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Roles.Administrator);

                return new Response_ApiUserRegisterDto()
                {
                    isSuccess = true,
                    apiUser = user,
                };
            }

            List<string> errors = new List<string>();

            foreach (var error in result.Errors)
            {
                errors.Add(error.Description.ToString());
            }

            return new Response_ApiUserRegisterDto()
            {
                isSuccess = false,
                Message = errors
            };
        }
    }
}
