using System.ComponentModel;

namespace BlogWebAPI.Models.Entities
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Permalink { get; set; }
        public Category Category { get; set; }
        public FileData ImageData { get; set; } 
        public string Excerpt { get; set; }
        public string Content { get; set; }
        public bool IsFeature { get; set; }
        public int Views { get; set; }
        public List<UserComment> UserComments { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }

    }

    public enum Status
    {
        @new, old
    }
}
