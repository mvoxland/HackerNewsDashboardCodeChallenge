using HackerNewsDashboard.Common.DTO;
using HackerNewsDashboard.Common.Models;
using HackerNewsDashboard.Data;
using HackerNewsDashboard.Data.Contexts;
using HackerNewsDashboard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;

namespace HackerNewsDashboard.Controllers
{
    [ApiController]
    [Route("api")]
    public class HackerNewsDashboardController : ControllerBase
    {

        private readonly ILogger<HackerNewsDashboardController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly HackerNewsDBContext _context;
        private readonly HttpClient _httpClient;

        public HackerNewsDashboardController(
            ILogger<HackerNewsDashboardController> logger, 
            UserManager<User> userManager,
            ITokenService tokenService,
            HackerNewsDBContext context,
            HttpClient httpClient)
        {
            _logger = logger;
            _userManager = userManager;
            _tokenService = tokenService;
            _context = context;
            _httpClient = httpClient;
        }

        #region "proxy"
        [Authorize]
        [HttpGet("proxy/item/{id:int}")]
        public async Task<ProxyItem?> GetProxyItem(int id)
        {
            if (id == 0) return null;

            try
            {
                var returned = await _httpClient.GetFromJsonAsync<ProxyItem>("item/" + id + ".json");
                return returned;
            }
            catch
            {
                return null;
            }
        }

        [Authorize]
        [HttpGet("proxy/user/{userId}")]
        public async Task<ProxyUser?> GetProxyUser(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;

            try
            {
                var returned = await _httpClient.GetFromJsonAsync<ProxyUser>("user/" + userId + ".json");
                return returned;
            }
            catch
            {
                return null;
            }
        }

        async Task<IEnumerable<int>?> GetSimpleProxy(string simpleProxy)
        {
            try
            {
                var returned = await _httpClient.GetFromJsonAsync<IEnumerable<int>>(simpleProxy + ".json");
                return returned;
            }
            catch
            {
                return null;
            }
        }

        [Authorize]
        [HttpGet("proxy/topstories")]
        public async Task<IEnumerable<int>?> GetProxyTopStories()
        {
            var res = await GetSimpleProxy("topstories");
            return res;
        }

        [Authorize]
        [HttpGet("proxy/newstories")]
        public async Task<IEnumerable<int>?> GetProxyNewStories()
        {
            var res = await GetSimpleProxy("newstories");
            return res;
        }

        [Authorize]
        [HttpGet("proxy/beststories")]
        public async Task<IEnumerable<int>?> GetProxyBestStories()
        {
            var res = await GetSimpleProxy("beststories");
            return res;
        }

        [Authorize]
        [HttpGet("proxy/askstories")]
        public async Task<IEnumerable<int>?> GetProxyAskStories()
        {
            var res = await GetSimpleProxy("askstories");
            return res;
        }

        [Authorize]
        [HttpGet("proxy/showstories")]
        public async Task<IEnumerable<int>?> GetProxyShowStories()
        {
            var res = await GetSimpleProxy("showstories");
            return res;
        }

        [Authorize]
        [HttpGet("proxy/jobstories")]
        public async Task<IEnumerable<int>?> GetProxyJobStories()
        {
            var res = await GetSimpleProxy("jobstories");
            return res;
        }
        #endregion

        #region "auth"
        [HttpPost("register")]
        public async Task<IActionResult> PostRegister([FromBody] Register model)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return BadRequest("You already have an account, please login instead.");
                }

                User user = new()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username,
                    EmailConfirmed = true
                };

                var createUserResult = await _userManager.CreateAsync(user, model.Password);
                if (createUserResult.Succeeded == false)
                {
                    var errors = createUserResult.Errors.Select(e => e.Description);
                    _logger.LogError($"Failed to create user: {string.Join(", ", errors)}");
                    return BadRequest("Failed to create user.");
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
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return BadRequest("Unrecognized User.");
                }
                bool isValidPassword = await _userManager.CheckPasswordAsync(user, model.Password);
                if (isValidPassword == false)
                {
                    return Unauthorized();
                }

                List<Claim> authClaims = [new(ClaimTypes.Name, user.Email!), new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())];

                var token = _tokenService.GenerateAccessToken(authClaims);

                string refreshToken = _tokenService.GenerateRefreshToken();

                //save record in database for the refresh token
                var tokenInfo = _context.TokenInfo.FirstOrDefault(a => a.Email == user.Email);

                if (tokenInfo == null)
                {
                    var ti = new TokenInfo
                    {
                        Email = user.Email!,
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
                var email = principal.Identity?.Name;

                var tokenInfo = _context.TokenInfo.SingleOrDefault(u => u.Email == email);
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
        #endregion
    }
}
