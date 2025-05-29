using Microsoft.EntityFrameworkCore;
using NovelWorld.Dbcontexts;
using NovelWorld.Dtos.Product.Novel;
using NovelWorld.Entities.Auth;
using NovelWorld.Exceptions;
using NovelWorld.Services.Interfaces.Auth;
using NovelWorld.Utils;

namespace NovelWorld.Services.Implements.Auth
{
    public class UserFollowService : IUserFollowService
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserFollowService(ILogger<UserFollowService> logger, ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task FollowUserAsync(int targetUserId)
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);

            if (currentUserId == targetUserId)
            {
                throw new UserFriendlyException("Bạn không thể theo dõi chính mình!");
            }

            bool alreadyFollowing = await _dbContext.UserFollows.AnyAsync(uf => uf.FollowerId == currentUserId && uf.FollowingId == targetUserId);

            if (alreadyFollowing)
            {
                throw new UserFriendlyException("Bạn đã theo dõi người này!");
            }

            var follow = new UserFollow
            {
                FollowerId = currentUserId,
                FollowingId = targetUserId
            };

            await _dbContext.UserFollows.AddAsync(follow);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UnfollowUserAsync(int targetUserId)
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);

            var follow = await _dbContext.UserFollows
                .FirstOrDefaultAsync(uf => uf.FollowerId == currentUserId && uf.FollowingId == targetUserId);

            if (follow == null)
            {
                throw new UserFriendlyException("Bạn chưa theo dõi người này!");
            }

            _dbContext.UserFollows.Remove(follow);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> GetFollowersCountAsync(int userId)
        {
            return await _dbContext.UserFollows.CountAsync(uf => uf.FollowingId == userId);
        }

        public async Task<int> GetFollowingCountAsync(int userId)
        {
            return await _dbContext.UserFollows.CountAsync(uf => uf.FollowerId == userId);
        }

        public async Task<List<AuthorDto>> GetUnfollowedAuthorsAsync()
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);

            // Lấy danh sách ID của các tác giả mà user đã theo dõi
            var followedAuthorIds = await _dbContext.UserFollows
                .Where(uf => uf.FollowerId == currentUserId)
                .Select(uf => uf.FollowingId)
                .ToListAsync();

            // Lấy danh sách tác giả chưa được theo dõi (ngoại trừ chính user)
            var authors = await _dbContext.Users
                .Where(u => !followedAuthorIds.Contains(u.UserId) && u.UserId != currentUserId)
                .ToListAsync();

            // Lấy số lượng followers và following
            var authorDtos = new List<AuthorDto>();
            foreach (var author in authors)
            {
                int followersCount = await GetFollowersCountAsync(author.UserId);
                int followingCount = await GetFollowingCountAsync(author.UserId);
                authorDtos.Add(new AuthorDto
                {
                    Id = author.UserId,
                    DisplayName = author.DisplayName,
                    Image = UploadFile.GetFileUrl(author.Image, _httpContextAccessor),
                    FollowersCount = followersCount,
                    FollowingCount = followingCount
                });
            }
            return authorDtos.ToList();
        }

        public async Task<List<AuthorDto>> GetFollowedAuthorsAsync()
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);

            // Lấy danh sách ID của các tác giả mà user đang theo dõi
            var followedAuthorIds = await _dbContext.UserFollows
                .Where(uf => uf.FollowerId == currentUserId)
                .Select(uf => uf.FollowingId)
                .ToListAsync();

            // Lấy danh sách thông tin tác giả đã theo dõi
            var authors = await _dbContext.Users
                .Where(u => followedAuthorIds.Contains(u.UserId))
                .ToListAsync();

            // Lấy số lượng followers và following
            var authorDtos = new List<AuthorDto>();
            foreach (var author in authors)
            {
                int followersCount = await GetFollowersCountAsync(author.UserId);
                int followingCount = await GetFollowingCountAsync(author.UserId);
                authorDtos.Add(new AuthorDto
                {
                    Id = author.UserId,
                    DisplayName = author.DisplayName,
                    Image = UploadFile.GetFileUrl(author.Image, _httpContextAccessor),
                    FollowersCount = followersCount,
                    FollowingCount = followingCount
                });
            }
            return authorDtos.ToList();
        }

        public async Task<List<AuthorDto>> GetFollowersAsync()
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);

            // Lấy danh sách ID của các tác giả đang theo dõi user hiện tại
            var followerIds = await _dbContext.UserFollows
                .Where(uf => uf.FollowingId == currentUserId)
                .Select(uf => uf.FollowerId)
                .ToListAsync();

            // Lấy danh sách thông tin tác giả đang theo dõi
            var followers = await _dbContext.Users
                .Where(u => followerIds.Contains(u.UserId))
                .ToListAsync();

            // Lấy số lượng followers và following
            var followerDtos = new List<AuthorDto>();
            foreach (var follower in followers)
            {
                int followersCount = await GetFollowersCountAsync(follower.UserId);
                int followingCount = await GetFollowingCountAsync(follower.UserId);
                followerDtos.Add(new AuthorDto
                {
                    Id = follower.UserId,
                    DisplayName = follower.DisplayName,
                    Image = UploadFile.GetFileUrl(follower.Image, _httpContextAccessor),
                    FollowersCount = followersCount,
                    FollowingCount = followingCount
                });
            }
            return followerDtos.ToList();
        }

        public async Task<bool> IsFollowingUserAsync(int targetUserId)
        {
            int currentUserId = CommonUntils.GetCurrentUserId(_httpContextAccessor);

            // Trả về true nếu người dùng hiện tại đang theo dõi targetUserId
            return await _dbContext.UserFollows
                .AnyAsync(uf => uf.FollowerId == currentUserId && uf.FollowingId == targetUserId);
        }
    }
}
