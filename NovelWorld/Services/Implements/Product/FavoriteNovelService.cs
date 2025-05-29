using Microsoft.EntityFrameworkCore;
using NovelWorld.Dbcontexts;
using NovelWorld.Entities.Product;
using NovelWorld.Exceptions;
using NovelWorld.Services.Implements.Auth;
using NovelWorld.Services.Interfaces.Product;
using NovelWorld.Utils;

namespace NovelWorld.Services.Implements.Product
{
    public class FavoriteNovelService : IFavoriteNovelService
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FavoriteNovelService(ILogger<FavoriteNovelService> logger, ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddFavoriteNovelAsync(int novelId)
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);
            var favoriteNovel = await _dbContext.FavoriteNovels
                .FirstOrDefaultAsync(ft => ft.UserId == currentUserId && ft.NovelId == novelId);
            if (favoriteNovel != null)
            {
                throw new UserFriendlyException("Novel này đã nằm trong danh sách yêu thích của bạn!");
            }
            var newfavoriteNovel = new FavoriteNovel
            {
                UserId = currentUserId,
                NovelId = novelId
            };
            await _dbContext.FavoriteNovels.AddAsync(newfavoriteNovel);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveFavoriteNovelAsync(int novelId)
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);
            var favoriteNovel = await _dbContext.FavoriteNovels
                .FirstOrDefaultAsync(ft => ft.UserId == currentUserId && ft.NovelId == novelId);
            if (favoriteNovel == null)
            {
                throw new UserFriendlyException("Novel này không nằm trong danh sách yêu thích của bạn!");
            }
            _dbContext.FavoriteNovels.Remove(favoriteNovel);
            await _dbContext.SaveChangesAsync();
        }

        // Đếm số lượng người yêu thích
        public async Task<int> GetFavoriteCountAsync(int novelId)
        {
            return await _dbContext.FavoriteNovels.CountAsync(f => f.NovelId == novelId);
        }

        public async Task<bool> IsFavoriteNovelAsync(int novelId)
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);

            return await _dbContext.FavoriteNovels  
                .AnyAsync(ft => ft.UserId == currentUserId && ft.NovelId == novelId);
        }
    }
}
