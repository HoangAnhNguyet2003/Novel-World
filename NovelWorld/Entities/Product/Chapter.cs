using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NovelWorld.Entities.Product
{
    public class Chapter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChapId { get; set; }
        public int ChapNumber { get; set; }
        public string ChapTitle { get; set; }
        public string ChapContent { get; set; }

        [ForeignKey("Novel")]
        public int NovelId { get; set; }
        public Novel Novel { get; set; }
    }
}
