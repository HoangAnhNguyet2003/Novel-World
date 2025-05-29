using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovelWorld.Services.Interfaces.Product;

namespace NovelWorld.Controllers.Product
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteNovelController : ApiControllerBase
    {
        private readonly IFavoriteNovelService _favoriteNovelService;

        public FavoriteNovelController(ILogger<FavoriteNovelController> logger, IFavoriteNovelService favoriteNovelService) : base(logger)
        {
            _favoriteNovelService = favoriteNovelService;
        }

        [HttpPost("add/{novelId}")]
        public async Task<IActionResult> AddFavoriteNovel(int novelId)
        {
            try
            {
                await _favoriteNovelService.AddFavoriteNovelAsync(novelId);
                return Ok(new { message = "Novel added to favorites successfully!" });
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [HttpDelete("remove/{novelId}")]
        public async Task<IActionResult> RemoveFavoriteNovel(int novelId)
        {
            try
            {
                await _favoriteNovelService.RemoveFavoriteNovelAsync(novelId);
                return Ok(new { message = "Novel removed from favorites successfully!" });
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [HttpGet("is-favorite/{novelId}")]
        public async Task<IActionResult> IsFavoriteNovel(int novelId)
        {
            try
            {
                bool isFavorite = await _favoriteNovelService.IsFavoriteNovelAsync(novelId);
                return Ok(new { isFavorite });
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

    }
}
