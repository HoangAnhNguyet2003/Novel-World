using NovelWorld.Dtos.Product.Chapter;
using NovelWorld.Entities.Product;

namespace NovelWorld.Services.Interfaces.Product
{
    public interface IChapterService
    {
        Task<int> GetChapCountAsync(int novelId);
        Task AddChapterAsync(AddChapterDto input);
        Task UpdateChapterAsync(UpdateChapterDto input);
        Task DeleteChapterAsync(int chapId);
        Task<ChapterDto> GetChapterByIdAsync(int chapId);
        Task<List<ChapterDto>> GetChaptersByNovelIdAsync(int novelId);
        Task DeleteAllChaptersByNovelIdAsync(int novelId);


    }
}
