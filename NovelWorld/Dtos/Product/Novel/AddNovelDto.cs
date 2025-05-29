using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NovelWorld.Dtos.Product.Novel
{
    public class AddNovelDto
    {
        private string _title;

        [Required]
        [StringLength(30, ErrorMessage = "Novel title must be between 3 and 30 characters long.", MinimumLength = 3)]
        public string Title
        {
            get => _title;
            set => _title = value?.Trim();
        }

        [Required(ErrorMessage = "Novel image is required.")]
        public IFormFile Image { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public string Type { get; set; } // thể loại        
    }
}