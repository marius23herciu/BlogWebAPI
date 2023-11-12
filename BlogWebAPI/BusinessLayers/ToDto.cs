using BlogWebAPI.Data;
using BlogWebAPI.Models.DTOs;
using BlogWebAPI.Models.Entities;

namespace BlogWebAPI.BusinessLayers
{
    public class ToDto
    {
        private readonly BlogDbContext dbContext;

        public ToDto(BlogDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<PostDto> PostToDto(Post post)
        {
            var postDto = new PostDto
            {
                Id = post.Id,
                Title= post.Title,
                Permalink = post.Permalink,
                Category = post.Category.Name,
                PostImgPath = post.ImageData.FileName,
                Excerpt = post.Excerpt,
                Content = post.Content,
                IsFeatured = post.IsFeature,
                UserComments = post.UserComments,
                Views = post.Views,
                Status = post.Status,
                CreatedAt = post.CreatedAt,
            };
            return postDto;
        }

    }
}
