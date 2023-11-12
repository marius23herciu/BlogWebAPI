namespace BlogWebAPI.Models.DTOs
{
    public class PostFromFrontend
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Permalink { get; set; }
        public string CategoryName { get; set; }
        public IFormFile Image { get; set; }
        public string Excerpt { get; set; }
        public string Content { get; set; }
    }
}
