using Common.Dto;
using Domain.Entities;
using EndPoint.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EndPoint.Api.Controllers
{
    [ApiController]
    public class AccountingController : ControllerBase
    {
        private readonly ILogger<AccountingController> _logger;

        private readonly SignInManager<MyUser> _signInManager;
        private readonly UserManager<MyUser> _userManager;
        private readonly RoleManager<MyRole> _roleManager;
        private readonly IConfiguration _configuration;
        public AccountingController(
            ILogger<AccountingController> logger,
            SignInManager<MyUser> signInManager,
            UserManager<MyUser> userManager,
            RoleManager<MyRole> roleManager,
            IConfiguration configuration)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("api/Accounting/Login")]
        public async Task<IActionResult> Login(LoginModel request)
        {
            try
            {
                var result = _signInManager.PasswordSignInAsync(request.Username, request.Password, false, true).Result;

                if (result.Succeeded)
                {
                    var user = _userManager.FindByNameAsync(request.Username).Result;
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var token = GetToken(authClaims);

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }
                else
                {
                    if (result.IsLockedOut)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, "اکانت شما به دلیل پنج بار ورود ناموفق به مدت پنج دقیق قفل شده است");
                    }

                    if (result.IsNotAllowed)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, "اکانت شما توسط مدیر قفل شده است");
                    }

                    return StatusCode(StatusCodes.Status400BadRequest, "رمزعبور یا نام کاربری اشتباه است");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stopped program because of exception");

                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
