using HackerNewsDashboard.API.Data.Contexts;
using HackerNewsDashboard.API.Services;
using HackerNewsDashboard.API.Data;
using HackerNewsDashboard.Common.DTO;
using HackerNewsDashboard.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HackerNewsDashboard.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class HackerNewsDashboardController : ControllerBase
    {

        private readonly ILogger<HackerNewsDashboardController> _logger;
        private readonly UserManager<User> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;
        private readonly HackerNewsDBContext _context;

        public HackerNewsDashboardController(
            ILogger<HackerNewsDashboardController> logger, 
            UserManager<User> userManager,
            //RoleManager<IdentityRole> roleManager,
            IConfiguration config,
            ITokenService tokenService,
            HackerNewsDBContext context)
        {
            _logger = logger;
            _userManager = userManager;
            //_roleManager = roleManager;
            _config = config;
            _tokenService = tokenService;
            _context = context;
        }

        [HttpGet("test")]
        public IActionResult GetTest()
        {
            return Ok("Test");
        }

        [Authorize]
        [HttpGet("testAuthorization")]
        public IActionResult GetTestAuthorization()
        {
            return Ok("Test Authorization");
        }

        [HttpPost("register")]
        public async Task<IActionResult> PostRegister([FromBody] Register model)
        {
            try
            {
                var existingUser = await _userManager.FindByNameAsync(model.Email);
                if (existingUser != null)
                {
                    return BadRequest("You already have an account, please login instead.");
                }

                User user = new()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Email,
                    EmailConfirmed = true
                };

                var createUserResult = await _userManager.CreateAsync(user, model.Password);
                if (createUserResult.Succeeded == false)
                {
                    var errors = createUserResult.Errors.Select(e => e.Description);
                    _logger.LogError($"Failed to create user: {string.Join(", ", errors)}");
                    return BadRequest($"Failed to create user.");
                }

                return Created();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> PostLogin(Login model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    return BadRequest("Unrecognized User.");
                }
                bool isValidPassword = await _userManager.CheckPasswordAsync(user, model.Password);
                if (isValidPassword == false)
                {
                    return Unauthorized();
                }

                List<Claim> authClaims = [ new (ClaimTypes.Name, user.UserName!), new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())];

                var token = _tokenService.GenerateAccessToken(authClaims);

                string refreshToken = _tokenService.GenerateRefreshToken();

                //save record in database for the refresh token
                var tokenInfo = _context.TokenInfo.
                            FirstOrDefault(a => a.Username == user.UserName);

                if (tokenInfo == null)
                {
                    var ti = new TokenInfo
                    {
                        Username = user.UserName!,
                        RefreshToken = refreshToken,
                        ExpiredAt = DateTime.UtcNow.AddDays(7).ToString(),  
                    };
                    _context.TokenInfo.Add(ti);
                }
                else
                {
                    tokenInfo.RefreshToken = refreshToken;
                    tokenInfo.ExpiredAt = DateTime.UtcNow.AddDays(7).ToString();
                }

                await _context.SaveChangesAsync();

                return Ok(new Token
                {
                    AccessToken = token,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> PostRefresh(Token tokenModel)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AccessToken);
                var username = principal.Identity?.Name;

                var tokenInfo = _context.TokenInfo.SingleOrDefault(u => u.Username == username);
                if (tokenInfo == null || tokenInfo.RefreshToken != tokenModel.RefreshToken || DateTime.Parse(tokenInfo.ExpiredAt) <= DateTime.UtcNow)
                {
                    return BadRequest("Invalid refresh token. Please login again.");
                }

                var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                tokenInfo.RefreshToken = newRefreshToken; // rotate the refresh token
                await _context.SaveChangesAsync();

                return Ok(new Token
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
