namespace NovelWorld.Dtos.Product.Novel
{
    public class NovelDto
    {
        public int NovelId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Image { get; set; }
        public string Type { get; set; }
        public int ChapCount { get; set; }
        public int Favorite { get; set; }
    }
}
