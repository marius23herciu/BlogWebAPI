using BlogWebAPI.Data;
using BlogWebAPI.Models.DTOs;
using BlogWebAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogWebAPI.BusinessLayers
{
    public class CommentsBusinessLayer
    {
        private readonly BlogDbContext dbContext;
        private readonly ToDto toDtos;

        public CommentsBusinessLayer(BlogDbContext dbContext, ToDto toDto)
        {
            this.dbContext = dbContext;
            this.toDtos = toDto;
        }

        public async Task<bool> CreateMainComment(Guid postId, string name, string comment)
        {
            var newComment = new UserComment()
            {
                Id = new Guid(),
                Name = name,
                Comment = comment,
                Approved = false,
                IsReply = false,
                ReplyFor = Guid.Empty,
                CreatedAt = DateTime.Now
            };

            var post = await dbContext.Posts.Where(i => i.Id == postId).Include(c => c.UserComments).FirstOrDefaultAsync();
            if (post != null)
            {
                post.UserComments.Add(newComment);
            }
            else
            {
                return false;
            }

            await dbContext.UserComments.AddAsync(newComment);
            await dbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> CreateReplyComment(Guid postId, Guid replyToId, string name, string comment)
        {
            var newComment = new UserComment()
            {
                Id = new Guid(),
                Name = name,
                Comment = comment,
                Approved = false,
                IsReply = true,
                ReplyFor = replyToId,
                CreatedAt = DateTime.Now
            };

            var post = await dbContext.Posts.Where(i => i.Id == postId).Include(c => c.UserComments).FirstOrDefaultAsync();
            if (post != null)
            {
                post.UserComments.Add(newComment);
            }
            else
            {
                return false;
            }

            await dbContext.UserComments.AddAsync(newComment);
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<UserComment>> GetUnapprovedComments()
        {
            var unapprovedComments = await dbContext.UserComments.Where(a => a.Approved == false)
                .OrderByDescending(a => a.CreatedAt).ToListAsync();

            return unapprovedComments;
        }

        public async Task<List<UserComment>> GetMainCommentsForPostId(Guid postId)
        {
            var post = await dbContext.Posts.Where(i=>i.Id==postId).Include(c=>c.UserComments).FirstOrDefaultAsync();
            if (post==null)
            {
                return null;
            }
            var comments = post.UserComments.Where(a => a.Approved == true).Where(r=>r.IsReply==false)
                .OrderByDescending(c => c.CreatedAt).ToList();

            return comments;
        }
        public async Task<List<UserComment>> GetReplyCommentsForPostId(Guid postId)
        {
            var post = await dbContext.Posts.Where(i => i.Id == postId).Include(c => c.UserComments).FirstOrDefaultAsync();
            if (post == null)
            {
                return null;
            }
            var comments = post.UserComments.Where(a => a.Approved == true).Where(r => r.IsReply == true)
                .OrderBy(c => c.CreatedAt).ToList();

            return comments;
        }

        public async Task<bool> ApproveComment(Guid id)
        {
            var comment = await dbContext.UserComments.Where(i => i.Id == id).FirstOrDefaultAsync();

            if (comment == null)
            {
                return false;
            }

            comment.Approved = true;

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteComment(Guid id)
        {

            var commToDelete = dbContext.UserComments.Where(i => i.Id == id).FirstOrDefault();

            if (commToDelete == null)
            {
                return false;
            }

            dbContext.UserComments.Remove(commToDelete);
            await dbContext.SaveChangesAsync();

            return true;
        }

    }
}
