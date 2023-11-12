namespace BlogWebAPI.Models.DTOs
{
    public class ReplyCommentDto
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public Guid ReplyFor { get; set; }
    }
}
