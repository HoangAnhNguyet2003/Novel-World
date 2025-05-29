using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NovelWorld.Dtos.Product.Novel;
using NovelWorld.Services.Interfaces.Product;

namespace NovelWorld.Controllers.Product
{
    [Route("api/[controller]")]
    [ApiController]
    public class NovelController : ApiControllerBase
    {
        private readonly INovelService _novelService;
        public NovelController(ILogger<NovelController> logger, INovelService novelService) : base(logger)
        {
            _novelService = novelService;
        }


        [Authorize]
        [HttpPost("add-novel")]
        public async Task<IActionResult> CreateNovel([FromForm] AddNovelDto input)
        {
            try
            {
                await _novelService.CreateNovelAsync(input);
                return Ok(new { message = "Novel added successfully!" });
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }


        [Authorize]
        [HttpDelete("delete-novel/{novelId}")]
        public async Task<IActionResult> DeleteNovel(int novelId)
        {
            try
            {
                await _novelService.DeleteNovelAsync(novelId);
                return Ok(new { message = "Novel deleted successfully!" });
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [HttpGet("get-novel/{novelId}")]

        public async Task<IActionResult> GetNovel(int novelId)
        {
            try
            {
                var novel = await _novelService.GetNovelAsync(novelId);
                return Ok(novel);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [HttpGet("get-all-novels")]

        public async Task<IActionResult> GetAllNovel()
        {
            try
            {
                var novels = await _novelService.GetAllNovelsAsync();
                return Ok(novels);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [Authorize]
        [HttpPut("update-novel")]

        public async Task<IActionResult> UpdateNovel([FromForm] UpdateNovelDto input)
        {
            try
            {
                await _novelService.UpdateNovelAsync(input);
                return Ok(new { message = "Novel updated successfully!" });
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [HttpGet("get-artist/{novelId}")]
        public async Task<IActionResult> GetArtist(int novelId)
        {
            try
            {
                var artist = await _novelService.GetAuthorByNovelAsync(novelId);
                return Ok(artist);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [Authorize]
        [HttpGet("get-novel-from-following")]
        public async Task<IActionResult> GetNovelsFromFollowing()
        {
            try
            {
                var novels = await _novelService.GetNovelsFromFollowingAsync();
                return Ok(novels);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }
        
        [Authorize]
        [HttpGet("get-novel-from-nonfollowing")]
        public async Task<IActionResult> GetNovelsFromNonFollowing()
        {
            try
            {
                var novels = await _novelService.GetNovelsFromNotFollowingAsync();
                return Ok(novels);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [Authorize]
        [HttpGet("get-favorite-novels")]
        public async Task<IActionResult> GetFavoriteNovels()
        {
            try
            {
                var novels = await _novelService.GetFavoriteNovelsAsync();
                return Ok(novels);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [Authorize]
        [HttpGet("get-unfavorited-novels")]
        public async Task<IActionResult> GetUnfavoritedNovels()
        {
            try
            {
                var novels = await _novelService.GetUnfavoritedNovelsAsync();
                return Ok(novels);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [Authorize]
        [HttpGet("get-my-novels")]
        public async Task<IActionResult> GetMyNovels()
        {
            try
            {
                var novels = await _novelService.GetMyNovelsAsync();
                return Ok(novels);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [Authorize]
        [HttpGet("get-novels-by-user/{userId}")]
        public async Task<IActionResult> GetNovelsByUserId(int userId)
        {
            try
            {
                var novels = await _novelService.GetNovelsByUserIdAsync(userId);
                return Ok(novels);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }
    }
}
