using NovelWorld.Entities.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NovelWorld.Entities.Product

{
    public class Novel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NovelId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Type { get; set; }
        // Mối quan hệ 1-1 với User (người đăng)
        [ForeignKey("User")]
        public int AuthorId { get; set; }
        public User Author { get; set; }
        public ICollection<FavoriteNovel> Favorite { get; set; } = new List<FavoriteNovel>();
        public ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();



    }
}
