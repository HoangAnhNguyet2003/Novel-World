using System.ComponentModel.DataAnnotations;

namespace NovelWorld.Dtos.Product.Chapter
{
    public class AddChapterDto
    {
        [Required(ErrorMessage = "Chapter Number is required.")]
        public int ChapNumber { get; set; }
        private string _chapTitle;

        [Required]
        [StringLength(30, ErrorMessage = "Chap title must be between 3 and 30 characters long.", MinimumLength = 3)]
        public string ChapTitle
        {
            get => _chapTitle;
            set => _chapTitle = value?.Trim();
        }

        [Required(ErrorMessage = "Chap content is required.")]
        public IFormFile ChapContent { get; set; }

        [Required]
        public int NovelId { get; set; }
    }
}
