using BlogWebAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogWebAPI.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<UserComment> UserComments { get; set; }
        public DbSet<FileData> FilesData { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
