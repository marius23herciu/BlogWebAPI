using BlogWebAPI.BusinessLayers;
using BlogWebAPI.Data;
using BlogWebAPI.Models.DTOs;
using BlogWebAPI.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly PostsBusinessLayer postsBusinessLayer;

        public PostsController(PostsBusinessLayer postsBusinessLayer)
        {
            this.postsBusinessLayer = postsBusinessLayer;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllPosts()
        {
            var allPosts = await postsBusinessLayer.GetAllPosts();
            return Ok(allPosts);
        }

        [HttpGet]
        [Route("post-{id}-{reload}")]
        public async Task<ActionResult> GetPost(Guid id, bool reload)
        {
            var post = await postsBusinessLayer.GetPost(id, reload);
            return Ok(post);
        }

        [HttpGet]
        [Route("by-{category}")]
        public async Task<ActionResult> GetPostsByCategory([FromRoute] string category)
        {
            var posts = await postsBusinessLayer.GetPostsByCategory(category);
            return Ok(posts);
        }

        [HttpGet]
        [Route("similar-posts-{category}-without-{postId}")]
        public async Task<ActionResult> Get4SimilarPostsByCategory([FromRoute] string category, [FromRoute] Guid postId)
        {
            var posts = await postsBusinessLayer.Get4SimilarPostsByCategory(category, postId);
            return Ok(posts);
        }

        [HttpGet]
        [Route("{searchString}")]
        public async Task<ActionResult> GetPostsBySearch([FromRoute] string searchString)
        {
            var posts = await postsBusinessLayer.GetPostsBySearch(searchString);
            return Ok(posts);
        }

        [HttpGet]
        [Route("featured")]
        public async Task<ActionResult> GetFeaturedPosts()
        {
            var featuredPosts = await postsBusinessLayer.GetFeaturedPosts();
            return Ok(featuredPosts);
        }

        [HttpGet]
        [Route("latest")]
        public async Task<ActionResult> GetLatestPosts()
        {
            var featuredPosts = await postsBusinessLayer.GetLatestPosts();
            return Ok(featuredPosts);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> CreatePost([FromForm] PostFromFrontend postDto)
        {
            if (await postsBusinessLayer.CreatePost(postDto))
            {
                return Ok(true);
            }
            else
            {
                return BadRequest(false);
            }
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> EditPost([FromForm] PostFromFrontend postDto)
        {
            if (await postsBusinessLayer.EditPost(postDto.Id, postDto))
            {
                return Ok(true);
            }
            else
            {
                return BadRequest(false);
            }
        }

        [HttpPut]
        [Route("{id}-mark-featured")]
        public async Task<ActionResult> MarkIsFeatured([FromRoute] Guid id)
        {
            if (await postsBusinessLayer.MarkIsFeatured(id))
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }
        [HttpPut]
        [Route("{id}-unmark-featured")]
        public async Task<ActionResult> UnmarkIsFeatured([FromRoute] Guid id)
        {
            if (await postsBusinessLayer.UnmarkIsFeatured(id))
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeletePost([FromRoute] Guid id)
        {
            return Ok(await postsBusinessLayer.DeletePost(id));
        }
    }
}
