using NovelWorld.Dtos.Product.Novel;

namespace NovelWorld.Services.Interfaces.Auth
{
    public interface IUserFollowService
    {
        Task FollowUserAsync(int targetUserId);
        Task UnfollowUserAsync(int targetUserId);
        Task<int> GetFollowersCountAsync(int userId);
        Task<int> GetFollowingCountAsync(int userId);
        Task<List<AuthorDto>> GetFollowedAuthorsAsync();
        Task<List<AuthorDto>> GetUnfollowedAuthorsAsync();
        Task<List<AuthorDto>> GetFollowersAsync();
        Task<bool> IsFollowingUserAsync(int targetUserId);


    }
}
