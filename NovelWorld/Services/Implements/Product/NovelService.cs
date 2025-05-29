using Microsoft.EntityFrameworkCore;
using NovelWorld.Dbcontexts;
using NovelWorld.Dtos.Auth.UserDtos;
using NovelWorld.Dtos.Product.Novel;
using NovelWorld.Entities.Product;
using NovelWorld.Exceptions;
using NovelWorld.Services.Interfaces.Auth;
using NovelWorld.Services.Interfaces.Product;
using NovelWorld.Utils;

namespace NovelWorld.Services.Implements.Product
{
    public class NovelService : INovelService
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFavoriteNovelService _favoriteNovelService;
        private readonly IUserFollowService _userFollowService;
        private readonly IChapterService _chapterService;

        public object CommonUtils { get; private set; }

        public NovelService(ILogger<NovelService> logger, ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, IFavoriteNovelService favoriteNovelService, IUserFollowService userFollowService, IChapterService chapterService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _favoriteNovelService = favoriteNovelService;
            _userFollowService = userFollowService;
            _chapterService = chapterService;
        }

        public async Task CreateNovelAsync(AddNovelDto input)
        {
            // Kiểm tra xem tiểu thuyết đã tồn tại chưa
            if (_dbContext.Novels.Any(t => t.Title == input.Title))
            {
                throw new UserFriendlyException($"Novel \"{input.Title}\" already exists");
            }

            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);

            // Lấy tên hiển thị của người dùng hiện tại
            var currentUser = await _dbContext.Users
                .Where(u => u.UserId == currentUserId)
                .Select(u => u.DisplayName)
                .FirstOrDefaultAsync();

            string imagePath = null;

            // Lưu hình ảnh nếu có
            if (input.Image != null)
            {
                imagePath = await UploadFile.SaveFileAsync(input.Image, "Novels", "Images");
            }

            // Tạo đối tượng novel và thêm vào cơ sở dữ liệu
            var novel = new Novel
            {
                Title = input.Title,
                AuthorId = currentUserId,
                Image = imagePath,
                Type = input.Type, // Thêm thể loại
            };

            _dbContext.Novels.Add(novel);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteNovelAsync(int novelId)
        {
            var novel = await _dbContext.Novels.FirstOrDefaultAsync(n => n.NovelId == novelId);
            if (novel != null)
            {
                _chapterService.DeleteAllChaptersByNovelIdAsync(novelId).Wait(); // Xóa tất cả chương liên quan đến tiểu thuyết
                // Xóa hình ảnh và nội dung nếu có
                UploadFile.DeleteFile(novel.Image, "Novels", "Images");

                _dbContext.Novels.Remove(novel);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new UserFriendlyException("Novel not found");
            }
        }

        public async Task UpdateNovelAsync(UpdateNovelDto input)
        {
            string imagePath = null;

            if (input.Image != null)
            {
                imagePath = await UploadFile.SaveFileAsync(input.Image, "Novels", "Images");
            }

            var novel = await _dbContext.Novels.FirstOrDefaultAsync(n => n.NovelId == input.Id);
            if (novel != null)
            {
                if (await _dbContext.Novels
                    .AnyAsync(n => n.Title == input.Title && n.NovelId != input.Id))
                {
                    throw new UserFriendlyException("Novel already exists");
                }

                // Xóa hình ảnh cũ nếu có
                UploadFile.DeleteFile(novel.Image, "Novels", "Images");

                novel.Title = input.Title;
                novel.Image = imagePath;
                novel.Type = input.Type; // Thêm loại
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new UserFriendlyException("Novel not found");
            }
        }

        public async Task<NovelDto> GetNovelAsync(int novelId)
        {
            var novel = await _dbContext.Novels.Include(n => n.Author).FirstOrDefaultAsync(n => n.NovelId == novelId);
            if (novel == null)
            {
                throw new UserFriendlyException("Novel not found");
            }

            int favoriteCount = await _favoriteNovelService.GetFavoriteCountAsync(novelId);
            int chapCount = await _chapterService.GetChapCountAsync(novelId); // Lấy số chương
            return new NovelDto
            {
                Title = novel.Title,
                Author = novel.Author.DisplayName,
                Image = UploadFile.GetFileUrl(novel.Image, _httpContextAccessor),
                Type = novel.Type, // Thêm loại
                ChapCount = chapCount,
                Favorite = favoriteCount
            };
        }
        public async Task<List<NovelDto>> GetAllNovelsAsync()
        {
            var novels = await _dbContext.Novels
                .Include(n => n.Author)
                .ToListAsync();

            var novelDtos = new List<NovelDto>();
            foreach (var novel in novels)
            {
                int favoriteCount = await _favoriteNovelService.GetFavoriteCountAsync(novel.NovelId);
                int chapCount = await _chapterService.GetChapCountAsync(novel.NovelId); // Lấy số chương
                novelDtos.Add(new NovelDto
                {
                    Title = novel.Title,
                    Author = novel.Author.DisplayName,
                    Image = UploadFile.GetFileUrl(novel.Image, _httpContextAccessor),
                    Type = novel.Type,
                    ChapCount = chapCount,
                    Favorite = favoriteCount
                });
            }

            return novelDtos;
        }

        public async Task<AuthorDto> GetAuthorByNovelAsync(int novelId)
        {
            var novel = await _dbContext.Novels
                .Include(n => n.Author)
                .FirstOrDefaultAsync(n => n.NovelId == novelId);

            if (novel?.Author == null)
            {
                throw new UserFriendlyException("Novel or Author not found");
            }

            // Lấy số lượng followers và following song song để tối ưu tốc độ
            var followersTask = await _userFollowService.GetFollowersCountAsync(novel.Author.UserId);
            var followingTask = await _userFollowService.GetFollowingCountAsync(novel.Author.UserId);

            return new AuthorDto
            {
                Id = novel.Author.UserId,
                DisplayName = novel.Author.DisplayName,
                Image = UploadFile.GetFileUrl(novel.Author.Image, _httpContextAccessor),
                FollowersCount = followersTask,
                FollowingCount = followingTask
            };
        }

        public async Task<List<NovelDto>> GetNovelsFromFollowingAsync()
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);

            var followingIds = await _dbContext.UserFollows
                .Where(uf => uf.FollowerId == currentUserId)
                .Select(uf => uf.FollowingId)
                .ToListAsync();

            var novels = await _dbContext.Novels
                .Where(n => followingIds.Contains(n.AuthorId))
                .Include(n => n.Author) // Ensure Author is included
                .ToListAsync();

            var novelDtos = new List<NovelDto>();
            foreach (var novel in novels)
            {
                int favoriteCount = await _favoriteNovelService.GetFavoriteCountAsync(novel.NovelId);
                int chapCount = await _chapterService.GetChapCountAsync(novel.NovelId); // Lấy số chương
                novelDtos.Add(new NovelDto
                {
                    Title = novel.Title,
                    Author = novel.Author.DisplayName,
                    Image = UploadFile.GetFileUrl(novel.Image, _httpContextAccessor),
                    Type = novel.Type,
                    ChapCount = chapCount,
                    Favorite = favoriteCount
                });
            }

            return novelDtos;
        }

        public async Task<List<NovelDto>> GetNovelsFromNotFollowingAsync()
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);

            var followingIds = await _dbContext.UserFollows
                .Where(uf => uf.FollowerId == currentUserId)
                .Select(uf => uf.FollowingId)
                .ToListAsync();

            var novels = await _dbContext.Novels
                .Where(n => !followingIds.Contains(n.AuthorId) && n.AuthorId != currentUserId)
                .Include(n => n.Author) // Ensure Author is included
                .ToListAsync();

            var novelDtos = new List<NovelDto>();
            foreach (var novel in novels)
            {
                int favoriteCount = await _favoriteNovelService.GetFavoriteCountAsync(novel.NovelId);
                int chapCount = await _chapterService.GetChapCountAsync(novel.NovelId); // Lấy số chương
                novelDtos.Add(new NovelDto
                {
                    Title = novel.Title,
                    Author = novel.Author.DisplayName,
                    Image = UploadFile.GetFileUrl(novel.Image, _httpContextAccessor),
                    Type = novel.Type,
                    ChapCount = chapCount,
                    Favorite = favoriteCount
                });
            }

            return novelDtos;
        }

        public async Task<List<NovelDto>> GetFavoriteNovelsAsync()
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);

            var favoriteNovels = await _dbContext.FavoriteNovels
                .Where(fn => fn.UserId == currentUserId)
                .Include(fn => fn.Novel)
                .ThenInclude(n => n.Author)
                .ToListAsync();

            var novelDtos = new List<NovelDto>();
            foreach (var fn in favoriteNovels)
            {
                int favoriteCount = await _favoriteNovelService.GetFavoriteCountAsync(fn.Novel.NovelId);
                int chapCount = await _chapterService.GetChapCountAsync(fn.Novel.NovelId); // Lấy số chương
                novelDtos.Add(new NovelDto
                {
                    Title = fn.Novel.Title,
                    Author = fn.Novel.Author.DisplayName,
                    Image = UploadFile.GetFileUrl(fn.Novel.Image, _httpContextAccessor),
                    Type = fn.Novel.Type,
                    ChapCount = chapCount,
                    Favorite = favoriteCount
                });
            }

            return novelDtos;
        }

        public async Task<List<NovelDto>> GetMyNovelsAsync()
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);

            var novels = await _dbContext.Novels
                .Where(n => n.AuthorId == currentUserId)
                .Include(n => n.Author) // Ensure Author is included
                .ToListAsync();

            var novelDtos = new List<NovelDto>();
            foreach (var novel in novels)
            {
                int favoriteCount = await _favoriteNovelService.GetFavoriteCountAsync(novel.NovelId);
                int chapCount = await _chapterService.GetChapCountAsync(novel.NovelId); // Lấy số chương
                novelDtos.Add(new NovelDto
                {
                    Title = novel.Title,
                    Author = novel.Author.DisplayName,
                    Image = UploadFile.GetFileUrl(novel.Image, _httpContextAccessor),
                    Type = novel.Type,
                    ChapCount = chapCount,
                    Favorite = favoriteCount
                });
            }

            return novelDtos;
        }

        public async Task<List<NovelDto>> GetNovelsByUserIdAsync(int userId)
        {
            var novels = await _dbContext.Novels
                .Where(n => n.AuthorId == userId)
                .Include(n => n.Author) // Ensure Author is included
                .ToListAsync();

            var novelDtos = new List<NovelDto>();
            foreach (var novel in novels)
            {
                int favoriteCount = await _favoriteNovelService.GetFavoriteCountAsync(novel.NovelId);
                int chapCount = await _chapterService.GetChapCountAsync(novel.NovelId); // Lấy số chương
                novelDtos.Add(new NovelDto
                {
                    Title = novel.Title,
                    Author = novel.Author.DisplayName,
                    Image = UploadFile.GetFileUrl(novel.Image, _httpContextAccessor),
                    Type = novel.Type,
                    ChapCount = chapCount,
                    Favorite = favoriteCount
                });
            }

            return novelDtos;
        }

        public async Task<List<NovelDto>> GetUnfavoritedNovelsAsync()
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);

            // Lấy danh sách ID tiểu thuyết đã được yêu thích
            var favoriteNovelIds = await _dbContext.FavoriteNovels
                .Where(fn => fn.UserId == currentUserId)
                .Select(fn => fn.NovelId)
                .ToListAsync();

            // Lấy danh sách tiểu thuyết chưa có trong danh sách yêu thích
            var novels = await _dbContext.Novels
                .Where(n => !favoriteNovelIds.Contains(n.NovelId)) // Lọc các tiểu thuyết chưa được yêu thích
                .Include(n => n.Author) // Đảm bảo truy xuất dữ liệu Author
                .ToListAsync();

            var novelDtos = new List<NovelDto>();
            foreach (var novel in novels)
            {
                int favoriteCount = await _favoriteNovelService.GetFavoriteCountAsync(novel.NovelId);
                int chapCount = await _chapterService.GetChapCountAsync(novel.NovelId); // Lấy số chương
                novelDtos.Add(new NovelDto
                {
                    Title = novel.Title,
                    Author = novel.Author?.DisplayName ?? "Unknown", // Tránh lỗi null
                    Image = UploadFile.GetFileUrl(novel.Image, _httpContextAccessor),
                    Type = novel.Type,
                    ChapCount = chapCount,
                    Favorite = favoriteCount
                });
            }

            return novelDtos;
        }

    }
}
