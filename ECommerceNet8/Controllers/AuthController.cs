using ECommerceNet8.DTOs.ApiUserDtos.Request;
using ECommerceNet8.DTOs.ApiUserDtos.Response;
using ECommerceNet8.Models.AuthModels;
using ECommerceNet8.Repositories.AuthRepository;
using ECommerceNet8.Templates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ECommerceNet8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authReposiotry;
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ISendGridClient _sendGridClient;

        public AuthController(IAuthRepository authReposiotry,
            UserManager<ApiUser> userManager,
            IConfiguration configuration,
            ISendGridClient sendGridClient
            )
        {
            _authReposiotry = authReposiotry;
            _userManager = userManager;
            _configuration = configuration;
            _sendGridClient = sendGridClient;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<Response_ApiUserRegisterDto>> Register
            ([FromBody]Request_ApiUserRegisterDto request_ApiUserRegisterDto)
        {
            var userDto = await _authReposiotry.Register(request_ApiUserRegisterDto);

            if(userDto.isSuccess == false)
            {
                return BadRequest(new Response_ApiUserRegisterDto()
                {
                    isSuccess = false,
                    Message = userDto.Message
                });
            }

            //USER CONFIRMATION
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(userDto.apiUser);

            //https://localhost:8080/authentication/verifyemail/userid=YourUserId&code=GeneratedCode

            var callbackUrl = Request.Scheme + "://" + Request.Host
                + Url.Action("ConfirmEmail", "Auth",
                new { userId = userDto.apiUser.Id, code = code });

            var body = EmailTemplates.EmailLinkTemplate(callbackUrl);

            //SENDING EMAIL
            var result = await SendEmail(body, "test@gmail.com");

            string EmailMessage = result ? "Email sent" : "Email Failed To Send";

            return Ok(new Response_ApiUserRegisterDto()
            {
                isSuccess = true,
                Message = new List<string>
                {
                    "User Registered",
                    EmailMessage
                }
            });
        }

        [HttpPost]
        [Route("registerAdmin/{secretKey}")]
        public async Task<ActionResult<Response_ApiUserRegisterDto>> RegisterAdmin(
            [FromRoute]int secretKey, [FromBody]Request_ApiUserRegisterDto request)
        {
            var userDto = await _authReposiotry.RegisterAdmin(request, secretKey);
            if(userDto.isSuccess == false)
            {
                return BadRequest(
                    new Response_ApiUserRegisterDto()
                    {
                        isSuccess = false,
                        Message = userDto.Message
                    });
            }

            return Ok(new Response_ApiUserRegisterDto()
            {
                isSuccess = true,
                Message = new List<string>
                {
                    "Admin user created successfully",
                    "Admin user dont need to verify email"
                }
            });
        }


        [HttpGet]
        [Route("ConfirmEmail")]
        public async Task<ActionResult<Response_ApiUserConfirmEmail>> ConfirmEmail(
            string userId, string code)
        {
            if(userId == null || code == null)
            {
                return BadRequest(new Response_ApiUserConfirmEmail()
                {
                    isSuccess = false,
                    Message = "Wrong email confirmation link"
                });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return BadRequest(new Response_ApiUserConfirmEmail()
                {
                    isSuccess = false,
                    Message = "Wrong user ID provided"
                });
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            var status = result.Succeeded ? "Thank you for confirming email adress"
                : "Your email address is not confirmed, please try again later";

            return Ok(new Response_ApiUserConfirmEmail()
            {
                isSuccess = true,
                Message = status
            });
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<Response_LoginDto>> Login(
            [FromBody]Request_LoginDto userLogin)
        {
            var authResponse = await _authReposiotry.Login(userLogin);
            if(authResponse.Result == false)
            {
                return BadRequest(authResponse);
            }

            return Ok(authResponse);
        }

        //PRIVATE FUNCTIONS

        private async Task<bool> SendEmail(
            string body, 
            string email, 
            string subject = "Email Verification")
        {
            string fromEmail = _configuration.GetSection("SendGridEmailSettings")
                .GetValue<string>("FromEmail");
            string fromName = _configuration.GetSection("SendGridEmailSettings")
                .GetValue<string>("FromName");

            var msg = new SendGridMessage()
            {
                From = new EmailAddress(fromEmail, fromName),
                Subject = subject,
                HtmlContent = body
            };

            var emailToSend = email;

            msg.AddTo("vaceintech@gmail.com");

            var response = await _sendGridClient.SendEmailAsync(msg);

            return response.IsSuccessStatusCode;
        }
    }
}
