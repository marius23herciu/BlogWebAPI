using BlogWebAPI.Models.Entities;

namespace BlogWebAPI.Models.DTOs
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Permalink { get; set; }
        public string Category { get; set; }
        public string PostImgPath { get; set; }
        public string Excerpt { get; set; }
        public string Content { get; set; }
        public bool IsFeatured { get; set; }
        public int Views { get; set; }
        public List<UserComment> UserComments { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
