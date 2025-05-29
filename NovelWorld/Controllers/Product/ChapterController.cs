using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NovelWorld.Dtos.Product.Chapter;
using NovelWorld.Services.Interfaces.Product;

namespace NovelWorld.Controllers.Product
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChapterController : ApiControllerBase
    {
        private readonly IChapterService _chapterService;

        public ChapterController(ILogger<ChapterController> logger, IChapterService chapterService)
            : base(logger)
        {
            _chapterService = chapterService;
        }

        [HttpPost("add-chapter")]
        public async Task<IActionResult> AddChapter([FromForm] AddChapterDto input)
        {
            try
            {
                await _chapterService.AddChapterAsync(input);
                return Ok(new { message = "Chapter added successfully!" });
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [HttpPut("update-chapter")]
        public async Task<IActionResult> UpdateChapter([FromForm] UpdateChapterDto input)
        {
            try
            {
                await _chapterService.UpdateChapterAsync(input);
                return Ok(new { message = "Chapter updated successfully!" });
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [HttpDelete("delete-chapter/{chapId}")]
        public async Task<IActionResult> DeleteChapter(int chapId)
        {
            try
            {
                await _chapterService.DeleteChapterAsync(chapId);
                return Ok(new { message = "Chapter deleted successfully!" });
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [HttpGet("get-chapter/{chapId}")]
        public async Task<IActionResult> GetChapterById(int chapId)
        {
            try
            {
                var chapter = await _chapterService.GetChapterByIdAsync(chapId);
                return Ok(chapter);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }

        [HttpGet("get-chapters-by-novel/{novelId}")]
        public async Task<IActionResult> GetChaptersByNovelId(int novelId)
        {
            try
            {
                var chapters = await _chapterService.GetChaptersByNovelIdAsync(novelId);
                return Ok(chapters);
            }
            catch (Exception ex)
            {
                return ReturnException(ex);
            }
        }
    }
}
