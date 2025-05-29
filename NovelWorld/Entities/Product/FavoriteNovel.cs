using NovelWorld.Entities.Auth;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NovelWorld.Entities.Product
{
    public class FavoriteNovel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Mối quan hệ với User
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        // Mối quan hệ với Novel
        [ForeignKey("Novel")]
        public int NovelId { get; set; }
        public Novel Novel { get; set; }
    }
}
