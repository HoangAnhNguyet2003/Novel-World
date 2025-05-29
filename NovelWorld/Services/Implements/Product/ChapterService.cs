using Microsoft.EntityFrameworkCore;
using NovelWorld.Dbcontexts;
using NovelWorld.Dtos.Product.Chapter;
using NovelWorld.Entities.Product;
using NovelWorld.Exceptions;
using NovelWorld.Services.Interfaces.Product;
using NovelWorld.Utils;

namespace NovelWorld.Services.Implements.Product
{
    public class ChapterService : IChapterService
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ChapterService(ILogger<ChapterService> logger, ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> GetChapCountAsync(int novelId)
        {
            return await _dbContext.Chapters.CountAsync(f => f.NovelId == novelId);
        }

        public async Task AddChapterAsync(AddChapterDto input)
        {
            var novelExists = await _dbContext.Novels.AnyAsync(n => n.NovelId == input.NovelId);
            if (!novelExists)
            {
                throw new UserFriendlyException($"Novel \"{input.NovelId}\" doesn't exist.");
            }

            // Kiểm tra xem chương đã tồn tại chưa (ví dụ theo ChapterNumber trong cùng một Novel)
            var chapterExists = await _dbContext.Chapters.AnyAsync(c =>
                c.NovelId == input.NovelId && c.ChapNumber == input.ChapNumber);

            if (chapterExists)
            {
                throw new UserFriendlyException($"Chapter {input.ChapNumber} already exists for Novel {input.NovelId}.");
            }
            string contentPath = null;

            // Lưu hình ảnh nếu có
            if (input.ChapContent != null)
            {
                contentPath = await UploadFile.SaveFileAsync(input.ChapContent, "Novels", "Contents");
            }
            var chapter = new Chapter
            {
                ChapNumber = input.ChapNumber,
                ChapTitle = input.ChapTitle,
                ChapContent = contentPath,
                NovelId = input.NovelId
            };

            _dbContext.Chapters.Add(chapter);
            await _dbContext.SaveChangesAsync();

        }

        public async Task UpdateChapterAsync(UpdateChapterDto input)
        {
            var chapter = await _dbContext.Chapters.FindAsync(input.ChapId);
            if (chapter == null)
            {
                throw new UserFriendlyException($"Chapter with ID {input.ChapId} does not exist.");
            }
            // Cập nhật thông tin chương
            chapter.ChapNumber = input.ChapNumber;
            chapter.ChapTitle = input.ChapTitle;
            // Cập nhật nội dung chương nếu có
            if (input.ChapContent != null)
            {
                // Xóa file cũ nếu có
                UploadFile.DeleteFile(chapter.ChapContent, "Novels", "Contents");
                chapter.ChapContent = await UploadFile.SaveFileAsync(input.ChapContent, "Novels", "Contents");
            }
            await _dbContext.SaveChangesAsync();

        }

        public async Task DeleteChapterAsync(int chapId)
        {
            var chapter = await _dbContext.Chapters.FindAsync(chapId);
            if (chapter == null)
            {
                throw new UserFriendlyException($"Chapter with ID {chapId} does not exist.");
            }
            // Xóa file nội dung chương nếu có
            UploadFile.DeleteFile(chapter.ChapContent, "Novels", "Contents");
            _dbContext.Chapters.Remove(chapter);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ChapterDto> GetChapterByIdAsync(int chapId)
        {
            var chapter = await _dbContext.Chapters
                .Include(c => c.Novel) // Bao gồm thông tin Novel nếu cần
                .FirstOrDefaultAsync(c => c.ChapId == chapId);
            if (chapter == null)
            {
                throw new UserFriendlyException($"Chapter with ID {chapId} does not exist.");
            }

            return new ChapterDto
            {
                ChapId = chapter.ChapId,
                ChapNumber = chapter.ChapNumber,
                ChapTitle = chapter.ChapTitle,
                ChapContent = UploadFile.GetFileUrl(chapter.ChapContent, _httpContextAccessor)
            };

        }

        public async Task<List<ChapterDto>> GetChaptersByNovelIdAsync(int novelId)
        {
            var chapters = await _dbContext.Chapters
                .Where(c => c.NovelId == novelId)
                .OrderBy(c => c.ChapNumber) // Sắp xếp theo ChapNumber
                .ToListAsync();
            if (chapters == null || chapters.Count == 0)
            {
                throw new UserFriendlyException($"No chapters found for Novel with ID {novelId}.");
            }

            return chapters.Select(c => new ChapterDto
            {
                ChapId = c.ChapId,
                ChapNumber = c.ChapNumber,
                ChapTitle = c.ChapTitle,
                ChapContent = UploadFile.GetFileUrl(c.ChapContent, _httpContextAccessor)
            }).ToList();
        }

        public async Task DeleteAllChaptersByNovelIdAsync(int novelId)
        {
            var chapters = await _dbContext.Chapters
                .Where(c => c.NovelId == novelId)
                .ToListAsync();
            if (chapters.Count == 0)
            {
                throw new UserFriendlyException($"No chapters found for Novel with ID {novelId}.");
            }
            foreach (var chapter in chapters)
            {
                // Xóa file nội dung chương nếu có
                UploadFile.DeleteFile(chapter.ChapContent, "Novels", "Contents");
            }
            _dbContext.Chapters.RemoveRange(chapters);
            await _dbContext.SaveChangesAsync();
        }
    }
}
