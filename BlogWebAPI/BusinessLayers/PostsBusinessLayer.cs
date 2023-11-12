using BlogWebAPI.Data;
using BlogWebAPI.Models.DTOs;
using BlogWebAPI.Models.Entities;
using BlogWebAPI.Services.EmailSender;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Text;

namespace BlogWebAPI.BusinessLayers
{
    public class PostsBusinessLayer
    {
        private readonly BlogDbContext dbContext;
        private readonly ToDto toDtos;
        private readonly string AppDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        private static List<FileRecord> fileDB = new List<FileRecord>();
        private readonly IEmailSender emailSender;

        public PostsBusinessLayer(BlogDbContext dbContext, ToDto toDto, IEmailSender emailSender)
        {
            this.dbContext = dbContext;
            this.toDtos = toDto;
            this.emailSender = emailSender;
        }
        public async Task<List<PostDto>> GetAllPosts()
        {
            var featuredPosts = await dbContext.Posts.Include(i => i.ImageData).Include(c => c.Category).Include(c => c.UserComments)
                .Where(f => f.IsFeature == true).OrderByDescending(d => d.CreatedAt).ToListAsync();
            var unfeaturedPosts = await dbContext.Posts.Include(i => i.ImageData).Include(c => c.Category)
                .Where(f => f.IsFeature == false).OrderByDescending(d => d.CreatedAt).ToListAsync();

            List<PostDto> postsDto = new List<PostDto>();
            foreach (var post in featuredPosts)
            {
                var postDto = toDtos.PostToDto(post);
                postsDto.Add(await postDto);
            }
            foreach (var post in unfeaturedPosts)
            {
                var postDto = toDtos.PostToDto(post);
                postsDto.Add(await postDto);
            }
            return postsDto;
        }

        public async Task<PostDto> GetPost(Guid id, bool reload)
        {
            var post = await dbContext.Posts.Include(i => i.ImageData).Include(c => c.Category).Include(c => c.UserComments)
                .Where(i => i.Id == id).FirstOrDefaultAsync();

            if (post == null)
            {
                return new PostDto();
            }

            if (reload==false)
            {
                post.Views += 1;
            }
            
            await dbContext.SaveChangesAsync();
            var postDto = await toDtos.PostToDto(post);

            return postDto;
        }

        public async Task<bool> MarkIsFeatured(Guid id)
        {
            var post = await dbContext.Posts.Include(i => i.ImageData).Include(c => c.Category).Include(c => c.UserComments)
                .Where(i => i.Id == id).FirstOrDefaultAsync();

            if (post == null)
            {
                return false;
            }

            var featuredPosts = await dbContext.Posts.Include(i => i.ImageData).Include(c => c.Category).Where(f => f.IsFeature == true).ToListAsync();
            if (featuredPosts.Count >= 4)
            {
                return false;
            }

            post.IsFeature = true;

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnmarkIsFeatured(Guid id)
        {
            var post = await dbContext.Posts.Include(i => i.ImageData).Include(c => c.Category).Include(c => c.UserComments)
                .Where(i => i.Id == id).FirstOrDefaultAsync();

            if (post == null)
            {
                return false;
            }

            post.IsFeature = false;
            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EditPost(Guid id, PostFromFrontend postFromFrontend)
        {
            var post = await dbContext.Posts.Include(i => i.ImageData).Include(c => c.Category).Include(c => c.UserComments)
                .Where(i => i.Id == id).FirstOrDefaultAsync();

            if (post == null)
            {
                return false;
            }

            if (File.Exists(@post.ImageData.FilePath))
            {
                File.Delete(@post.ImageData.FilePath);
            }

            FileData fileData = new FileData();
            try
            {
                FileRecord file = await SaveFileAsync(postFromFrontend.Image);

                if (!string.IsNullOrEmpty(file.FilePath))
                {
                    fileData = await SaveToDB(file);
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }

            post.Title = postFromFrontend.Title;
            post.Permalink = postFromFrontend.Permalink;
            var categories = await dbContext.Categories.ToListAsync();
            var category = new Category();
            foreach (var categ in categories)
            {
                if (categ.Name == postFromFrontend.CategoryName)
                {
                    category = categ;
                    break;
                }
            }
            post.Category = category;
            post.ImageData = fileData;
            post.Excerpt = postFromFrontend.Excerpt;
            post.Content = postFromFrontend.Content;
            //post.CreatedAt = DateTime.Now;

            await dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<PostDto>> GetPostsBySearch(string searchString)
        {
            var posts = await dbContext.Posts.Include(i => i.ImageData).Include(c => c.Category).Include(c => c.UserComments)
                .Where(c => c.Title.Contains(searchString)).OrderByDescending(d => d.CreatedAt).ToListAsync();

            List<PostDto> postsDto = new List<PostDto>();
            foreach (var post in posts)
            {
                var postDto = toDtos.PostToDto(post);
                postsDto.Add(await postDto);
            }
            return postsDto;
        }
        public async Task<List<PostDto>> GetPostsByCategory(string category)
        {
            var posts = await dbContext.Posts.Include(i => i.ImageData).Include(c => c.Category)
                .Where(c => c.Category.Name == category).OrderByDescending(d => d.CreatedAt).ToListAsync();

            List<PostDto> postsDto = new List<PostDto>();
            foreach (var post in posts)
            {
                var postDto = toDtos.PostToDto(post);
                postsDto.Add(await postDto);
            }
            return postsDto;
        }

        public async Task<List<PostDto>> Get4SimilarPostsByCategory(string category, Guid postId)
        {
            var posts = await dbContext.Posts.Include(i => i.ImageData).Include(c => c.Category).Include(c => c.UserComments)
                .Where(c => c.Category.Name == category).Where(i => i.Id != postId)
                .OrderByDescending(d => d.CreatedAt).Take(4).ToListAsync();


            List<PostDto> postsDto = new List<PostDto>();
            foreach (var post in posts)
            {
                var postDto = toDtos.PostToDto(post);
                postsDto.Add(await postDto);
            }

            if (posts.Count < 4)
            {
                var extraPosts = await dbContext.Posts.Include(i => i.ImageData).Include(c => c.Category)
                .Where(c => c.Category.Name != category)
                .OrderByDescending(d => d.CreatedAt).Take(4-posts.Count).ToListAsync();

                foreach (var post in extraPosts)
                {
                    var postDto = toDtos.PostToDto(post);
                    postsDto.Add(await postDto);
                }
            }
            return postsDto;
        }

        public async Task<List<PostDto>> GetFeaturedPosts()
        {
            var featuredPosts = await dbContext.Posts.Include(i => i.ImageData).Include(c => c.Category).Include(c => c.UserComments)
                .Where(f => f.IsFeature == true).OrderByDescending(d => d.CreatedAt).ToListAsync();

            List<PostDto> postsDto = new List<PostDto>();
            foreach (var post in featuredPosts)
            {
                var postDto = toDtos.PostToDto(post);
                postsDto.Add(await postDto);
            }
            return postsDto;
        }
        public async Task<List<PostDto>> GetLatestPosts()
        {
            var latestPosts = await dbContext.Posts.Include(i => i.ImageData).Include(c => c.Category).Include(c => c.UserComments)
                .Where(f => f.IsFeature == false).OrderByDescending(d => d.CreatedAt).Take(6).ToListAsync();

            List<PostDto> postsDto = new List<PostDto>();
            foreach (var post in latestPosts)
            {
                var postDto = toDtos.PostToDto(post);
                postsDto.Add(await postDto);
            }
            return postsDto;
        }


        public async Task<bool> DeletePost (Guid id)
    {
            var post = await dbContext.Posts.Include(i => i.ImageData).Include(c => c.Category).Include(c => c.UserComments)
                .Where(i => i.Id == id).FirstOrDefaultAsync();

            if (post == null)
            {
                return false;
            }

            if (File.Exists(@post.ImageData.FilePath))
            {
                File.Delete(@post.ImageData.FilePath);
            }

            dbContext.Posts.Remove(post);
            await dbContext.SaveChangesAsync();
            return true;
    }

    public async Task<bool> CreatePost(PostFromFrontend postFromFrontend)
        {
            var checkCategory = await dbContext.Categories.Where(c => c.Name == postFromFrontend.CategoryName).FirstOrDefaultAsync();
            if (checkCategory == null)
            {
                checkCategory = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = postFromFrontend.CategoryName,
                };
                await dbContext.Categories.AddAsync(checkCategory);
            }




            FileData fileData = new FileData();
            try
            {
                FileRecord file = await SaveFileAsync(postFromFrontend.Image);

                if (!string.IsNullOrEmpty(file.FilePath))
                {
                    //file.AltText = model.AltText;
                    //file.Description = model.Description;


                    //Save to Inmemory object
                    //fileDB.Add(file);
                    //Save to SQL Server DB
                    fileData = await SaveToDB(file);
                    // return new HttpResponseMessage(HttpStatusCode.OK);
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }





            var newPost = new Post
            {
                Id = Guid.NewGuid(),
                Title = postFromFrontend.Title,
                Permalink = postFromFrontend.Permalink,
                Category = checkCategory,
                ImageData = fileData,
                Excerpt = postFromFrontend.Excerpt,
                Content = postFromFrontend.Content,
                IsFeature = false,
                UserComments = new List<UserComment>(),
                Views = 0,
                Status = Status.@new,
                CreatedAt = DateTime.Now,
            };

            await dbContext.Posts.AddAsync(newPost);
            await dbContext.SaveChangesAsync();

            var subscribers = await dbContext.Subscribers.ToListAsync();
            foreach (var subscriber in subscribers)
            {
                emailSender.SendNewPostEmailAsync(subscriber.Email, subscriber.Name);
            }
            
            return true;
        }

        private async Task<FileRecord> SaveFileAsync(IFormFile myFile)
        {
            FileRecord file = new FileRecord();
            if (myFile != null)
            {
                var serverPath = "C:\\Users\\seb\\Desktop\\Blog Angular + .Net\\globalAssets\\images";
                //if (!Directory.Exists(AppDirectory))
                 //   Directory.CreateDirectory(AppDirectory);
                if (!Directory.Exists(serverPath))
                   Directory.CreateDirectory(serverPath);

                var fileName = DateTime.Now.Ticks.ToString() + Path.GetExtension(myFile.FileName);
                //var path = Path.Combine(AppDirectory, fileName);
                var path = Path.Combine(serverPath, fileName);

                file.Id = fileDB.Count() + 1;
                file.FilePath = path;
                file.FileName = fileName;
                file.FileFormat = Path.GetExtension(myFile.FileName);
                file.ContentType = myFile.ContentType;

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await myFile.CopyToAsync(stream);
                }

                return file;
            }
            return file;
        }

        private async Task<FileData> SaveToDB(FileRecord record)
        {
            if (record == null)
                throw new ArgumentNullException($"{nameof(record)}");

            FileData fileData = new FileData();
            fileData.FilePath = record.FilePath;
            fileData.FileName = record.FileName;
            fileData.FileExtension = record.FileFormat;
            fileData.MimeType = record.ContentType;

            await dbContext.FilesData.AddAsync(fileData);
            await dbContext.SaveChangesAsync();
            return fileData;
        }
    }
}
