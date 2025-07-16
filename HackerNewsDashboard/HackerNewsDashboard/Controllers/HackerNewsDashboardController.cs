using HackerNewsDashboard.Common.DTO;
using HackerNewsDashboard.Data;
using HackerNewsDashboard.Data.Contexts;
using HackerNewsDashboard.Data.Models;
using HackerNewsDashboard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
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

        [Authorize]
        [HttpGet("bestStoriesPreviewList")]
        public async Task<StoriesWithCount?> GetBestStoriesPreviewList(int? skip, int? take)
        {
            if (take is null || take == 0 || skip is null) return null;

            try
            {
                var bestStoryIds = await _httpClient.GetFromJsonAsync<IEnumerable<int>>("beststories.json");
                
                if(bestStoryIds is null || !bestStoryIds.Any()) return null;
                if (skip < 0 || take < 0 || skip + take > bestStoryIds.Count()) return null;

                List<Task<ProxyItem?>> queries = new();
                foreach (var storyId in bestStoryIds.Skip(skip!.Value).Take(take!.Value))
                {
                    queries.Add(_httpClient.GetFromJsonAsync<ProxyItem>("item/" + storyId + ".json"));
                }
                await Task.WhenAll(queries);

                List<HNStory> bestStories = new();
                foreach (var queryResult in queries)
                {
                    if(queryResult?.Result is not null && queryResult.Result.Deleted != true && queryResult.Result.Dead != true)
                    {
                        var userComments = await _context.Comments.Where(c => c.ItemId == queryResult.Result.Id).ToListAsync();
                        var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Name)!);
                        var userRating = await _context.Ratings.Where(c => c.ItemId == queryResult.Result.Id && c.Username == user!.UserName).FirstOrDefaultAsync();
                        bestStories.Add(new HNStory()
                        {
                            Id = queryResult.Result.Id,
                            By = queryResult.Result.By,
                            Time = queryResult.Result.Time,
                            Kids = queryResult.Result.Kids,
                            Url = queryResult.Result.Url,
                            Score = queryResult.Result.Score,
                            Title = queryResult.Result.Title,
                            Descendants = queryResult.Result.Descendants,

                            UserComments = userComments?.Select(uc => new UserComment()
                            {
                                ItemId = uc.ItemId,
                                Username = uc.Username,
                                CommentText = uc.CommentText,
                                CommentDateTime = DateTime.Parse(uc.CommentDateTime, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal)
                            }),

                            UserRating = userRating == null ? null : new UserRating()
                            {
                                ItemId = userRating.ItemId,
                                Username = userRating.Username,
                                RatingStars = userRating.RatingStars,
                                RatingDateTime = DateTime.Parse(userRating.RatingDateTime, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal)
                            }
                        });
                    }
                }

                if(bestStories.Count > 0)
                    return new StoriesWithCount() { Stories = bestStories, Count = bestStoryIds.Count() };
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        [Authorize]
        [HttpPost("postComment")]
        public async Task<IActionResult> PostComment([FromBody] UserComment comment)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Name)!);
                Comment commentToAdd = new()
                {
                    ItemId = comment.ItemId,
                    Username = user!.UserName!,
                    CommentText = comment.CommentText,
                    CommentDateTime = DateTime.UtcNow.ToString()
                };

                _context.Add(commentToAdd);
                var res = await _context.SaveChangesAsync();
                
                if(res == 1)
                {
                    return Created();
                }
                else
                {
                    _logger.LogError($"PostComment: {res} rows affected");
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPost("postRating")]
        public async Task<IActionResult> PostRating([FromBody] UserRating rating)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Name)!);

                var existingRating = await _context.Ratings.Where(c => c.ItemId == rating.ItemId && c.Username == user!.UserName).FirstOrDefaultAsync();
                if (existingRating != null)
                {
                    existingRating.RatingStars = rating.RatingStars;
                    existingRating.RatingDateTime = DateTime.UtcNow.ToString();
                }
                else 
                {
                    Rating ratingToAdd = new()
                    {
                        ItemId = rating.ItemId,
                        Username = user!.UserName!,
                        RatingStars = rating.RatingStars,
                        RatingDateTime = DateTime.UtcNow.ToString()
                    };

                    _context.Add(ratingToAdd);
                }

                var res = await _context.SaveChangesAsync();

                if (res == 1)
                {
                    return Created();
                }
                else
                {
                    _logger.LogError($"PostComment: {res} rows affected");
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
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
                if (tokenInfo == null || tokenInfo.RefreshToken != tokenModel.RefreshToken || DateTime.Parse(tokenInfo.ExpiredAt, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal) <= DateTime.UtcNow)
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
