using NovelWorld.Dtos.Product.Novel;
using System.Threading.Tasks;

namespace NovelWorld.Services.Interfaces.Product
{
    public interface INovelService
    {
        Task CreateNovelAsync(AddNovelDto input);
        Task DeleteNovelAsync(int novelId);
        Task UpdateNovelAsync(UpdateNovelDto input);
        Task<NovelDto> GetNovelAsync(int novelId);
        Task<List<NovelDto>> GetAllNovelsAsync();

        Task<AuthorDto> GetAuthorByNovelAsync(int novelId);

        Task<List<NovelDto>> GetNovelsFromFollowingAsync();

        Task<List<NovelDto>> GetNovelsFromNotFollowingAsync();
        Task<List<NovelDto>> GetFavoriteNovelsAsync();
        Task<List<NovelDto>> GetUnfavoritedNovelsAsync();
        Task<List<NovelDto>> GetMyNovelsAsync();
        Task<List<NovelDto>> GetNovelsByUserIdAsync(int userId);
    }
}
