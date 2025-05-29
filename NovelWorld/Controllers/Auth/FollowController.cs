using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NovelWorld.Dtos.Product.Novel;
using NovelWorld.Services.Interfaces.Auth;

namespace NovelWorld.Controllers.Auth
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : ApiControllerBase
    {
        private readonly IUserFollowService _userFollowService;
        public FollowController(ILogger<FollowController> logger, IUserFollowService userFollowService) : base(logger)
        {
            _userFollowService = userFollowService;
        }

        [HttpPost("follow/{targetUserId}")]
        public async Task<IActionResult> FollowUser(int targetUserId)
        {
            try
            {
                await _userFollowService.FollowUserAsync(targetUserId);
                return Ok(new { message = "Đã theo dõi!" });
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }
        [HttpDelete("unfollow/{targetUserId}")]
        public async Task<IActionResult> UnfollowUser(int targetUserId)
        {
            try
            {
                await _userFollowService.UnfollowUserAsync(targetUserId);
                return Ok(new { message = "Đã bỏ theo dõi!" });
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [HttpGet("get-followed-authors")]
        public async Task<IActionResult> GetFollowedAuthors()
        {
            try
            {
                List<AuthorDto> authors = await _userFollowService.GetFollowedAuthorsAsync();
                return Ok(authors);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [HttpGet("get-unfollowed-authors")]
        public async Task<IActionResult> GetUnfollowedAuthors()
        {
            try
            {
                List<AuthorDto> authors = await _userFollowService.GetUnfollowedAuthorsAsync();
                return Ok(authors);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [HttpGet("get-followers")]
        public async Task<IActionResult> GetFollowers()
        {
            try
            {
                List<AuthorDto> followers = await _userFollowService.GetFollowersAsync();
                return Ok(followers);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [HttpGet("is-following/{targetUserId}")]
        public async Task<IActionResult> IsFollowingUser(int targetUserId)
        {
            try
            {
                bool isFollowing = await _userFollowService.IsFollowingUserAsync(targetUserId);
                return Ok(new { isFollowing });
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

    }
}
