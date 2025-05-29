using NovelWorld.Entities.Product;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NovelWorld.Entities.Auth
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public string DisplayName { get; set; }
        public int BirthYear { get; set; }
        public string Gender { get; set; }
        public string Image { get; set; }

        public ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();
        public ICollection<UserFollow> Following { get; set; } = new List<UserFollow>();
        public ICollection<Novel> Novels { get; set; } = new List<Novel>();
        public ICollection<FavoriteNovel> FavoriteNovels { get; set; } = new List<FavoriteNovel>();
    }

}
