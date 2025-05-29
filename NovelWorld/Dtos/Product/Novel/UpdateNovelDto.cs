using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NovelWorld.Dtos.Product.Novel
{
    public class UpdateNovelDto
    {
        public int Id { get; set; }

        private string _title;
        [StringLength(30, ErrorMessage = "Novel title must be between 3 and 30 characters long.", MinimumLength = 3)]
        public string Title
        {
            get => _title;
            set => _title = value?.Trim();
        }

        [Required(ErrorMessage = "Novel image is required.")]
        public IFormFile Image { get; set; }  // Hỗ trợ upload file ảnh

        private string _type;
        [Required(ErrorMessage = "Type is required.")]
        public string Type
        {
            get => _type;
            set => _type = value?.Trim();
        }
    }
}