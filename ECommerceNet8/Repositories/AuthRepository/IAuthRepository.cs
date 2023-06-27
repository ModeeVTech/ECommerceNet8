using ECommerceNet8.DTOs.ApiUserDtos.Request;
using ECommerceNet8.DTOs.ApiUserDtos.Response;

namespace ECommerceNet8.Repositories.AuthRepository
{
    public interface IAuthRepository
    {
        Task<Response_ApiUserRegisterDto> Register(Request_ApiUserRegisterDto userDto);
        Task<Response_ApiUserRegisterDto> RegisterAdmin(Request_ApiUserRegisterDto userDto,
            int secretKey);
    }
}
