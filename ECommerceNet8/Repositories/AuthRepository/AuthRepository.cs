using ECommerceNet8.Constants;
using ECommerceNet8.Data;
using ECommerceNet8.DTOs.ApiUserDtos.Request;
using ECommerceNet8.DTOs.ApiUserDtos.Response;
using ECommerceNet8.Models.AuthModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        public async Task<Response_LoginDto> Login(Request_LoginDto login)
        {
            bool isValidUser = false;
            var user = await _userManager.FindByEmailAsync(login.Email);
            if(user == null)
            {
                return new Response_LoginDto()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Wrong login credentials"
                    }
                };
            }

            if(user.EmailConfirmed == false)
            {
                return new Response_LoginDto()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Email needs to be confirmed"
                    }
                };
            }

            var token = await GenerateToken(user);
            var refreshToken = await CreateRefreshToken(user, token);

            return new Response_LoginDto()
            {
                Result = true,
                UserId = user.Id,
                Token = token,
                RefreshToken = refreshToken
            };
        }
        public async Task<Response_LoginDto> VerifyAndGenerateTokens(Request_TokenDto tokenDto)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(tokenDto.Token);

            var tokenIssuer = tokenContent.Issuer;
            if(tokenIssuer != _configuration["JwtSettings:Issuer"])
            {
                return new Response_LoginDto()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Token parameters are wrong"
                    }
                };
            }
            var tokenAudience = tokenContent.Audiences.ToList();

            if (!tokenAudience.Contains(_configuration["JwtSettings:Audience"]))
            {
                return new Response_LoginDto()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Token parameters are wrong"
                    }
                };
            }

            var userName = tokenContent.Claims.ToList()
                .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            var user  = await _userManager.FindByNameAsync(userName);
            if(user == null ||user.Id != tokenDto.UserId)
            {
                return new Response_LoginDto()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Token parameters are wrong"
                    }
                };
            }

            //REFRESH TOKEN VALIDATIONS
            var refreshTokenFromDb = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rf => rf.Token == tokenDto.RefreshToken);
            if(refreshTokenFromDb == null)
            {
                return new Response_LoginDto()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Refresh token is wrong"
                    }
                };
            }

            if(refreshTokenFromDb.JwtId != tokenContent.Id)
            {
                return new Response_LoginDto()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Refresh token is wrong"
                    }
                };
            }

            if(refreshTokenFromDb.ExpireDate  < DateTime.UtcNow)
            {
                return new Response_LoginDto()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        "Refresh token is wrong"
                    }
                };
            }

            var newToken = await GenerateToken(user);
            var newRefreshToken = await CreateRefreshToken(user, newToken);

            return new Response_LoginDto()
            {
                Result = true,
                Errors = new List<string>(),
                Token = newToken,
                RefreshToken = newRefreshToken,
                UserId = user.Id,
            };
        }
        public async Task<bool> LogoutDeleteRefreshToken(string userId)
        {
            var refreshToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.UserId == userId);
            if(refreshToken == null)
            {
                return true;
            }
            _dbContext.RefreshTokens.Remove(refreshToken);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        //PRIVATE FUNCTIONS

        private async Task<string> GenerateToken(ApiUser user)
        {
            //GET DATA READY
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(
                ClaimTypes.Role,
                role));

            var userClaims = await _userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
            }.Union(userClaims).Union(roleClaims);

            //GENETRATE TOKEN
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims = claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> CreateRefreshToken(ApiUser user, string token)
        {
            var existingRefreshToken = await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt=> rt.UserId == user.Id); 
            if(existingRefreshToken != null) 
            { 
                _dbContext.RefreshTokens.Remove(existingRefreshToken);
                await _dbContext.SaveChangesAsync();
            }

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityTokenHandler.ReadJwtToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = tokenContent.Id,
                Token = RandomStringGeneration(23),
                AddedDate = DateTime.UtcNow,
                ExpireDate = DateTime.UtcNow.AddMinutes(110),
                UserId = user.Id,
            };

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return refreshToken.Token;
        }

        private string RandomStringGeneration(int length) 
        {
            var random = new Random();
            var chars = "ABCDEFGHJKLMNOPQRSTUVWYZ123456789abvdefghklmnoprstuvwyz_";

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        
    }
}
