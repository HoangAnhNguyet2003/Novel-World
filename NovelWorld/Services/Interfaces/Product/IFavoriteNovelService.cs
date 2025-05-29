namespace NovelWorld.Services.Interfaces.Product
{
    public interface IFavoriteNovelService
    {
        Task AddFavoriteNovelAsync(int novelId);
        Task RemoveFavoriteNovelAsync(int novelId);
        Task<int> GetFavoriteCountAsync(int novelId);
        Task<bool> IsFavoriteNovelAsync(int novelId);

    }
}
