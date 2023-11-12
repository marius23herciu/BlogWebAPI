namespace BlogWebAPI.Models.Entities
{
    public class UserComment
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public bool Approved { get; set; }
        public bool IsReply { get; set; }
        public Guid ReplyFor { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
