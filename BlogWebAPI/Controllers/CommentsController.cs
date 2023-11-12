using BlogWebAPI.BusinessLayers;
using BlogWebAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly CommentsBusinessLayer commentsBusinessLayer;

        public CommentsController(CommentsBusinessLayer commentsBusinessLayer)
        {
            this.commentsBusinessLayer = commentsBusinessLayer;
        }

        [HttpPost("{postId}")]
        public async Task<ActionResult<bool>> CreateMainComment([FromRoute] Guid postId, CommentDto commentDto)
        {
            return Ok(await commentsBusinessLayer.CreateMainComment(postId, commentDto.Name, commentDto.Comment));
        }
        [HttpPost]
        [Route ("reply-at-{postId}")]
        public async Task<ActionResult<bool>> CreateReplyComment([FromRoute] Guid postId, ReplyCommentDto replyDto)
        {
            return Ok(await commentsBusinessLayer.CreateReplyComment(postId, replyDto.ReplyFor,  replyDto.Name, replyDto.Comment));
        }

        [HttpGet]
        [Route("unapproved-comments")]
        public async Task<IActionResult> GetUnapprovedComments()
        {
            return Ok(await commentsBusinessLayer.GetUnapprovedComments());
        }

        [HttpGet]
        [Route("main-for-{postId}")]
        public async Task<IActionResult> GetMainCommentsForPostId([FromRoute]Guid postId)
        {
            return Ok(await commentsBusinessLayer.GetMainCommentsForPostId(postId));
        }
        [HttpGet]
        [Route("replies-for-{postId}")]
        public async Task<IActionResult> GetReplyCommentsForPostId([FromRoute] Guid postId)
        {
            return Ok(await commentsBusinessLayer.GetReplyCommentsForPostId(postId));
        }

        [HttpPut]
        [Route("approve-{id}")]
        public async Task<ActionResult> MarkIsFeatured([FromRoute] Guid id)
        {
            if (await commentsBusinessLayer.ApproveComment(id))
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

        [HttpDelete]
        [Route("{commToDelete}")]
        public async Task<ActionResult<bool>> DeleteSubscriber([FromRoute] Guid commToDelete)
        {
            var subscriberDeleted = await commentsBusinessLayer.DeleteComment(commToDelete);
            if (subscriberDeleted == false)
            {
                return Ok(subscriberDeleted);
            }
            return Ok(subscriberDeleted);
        }
    }
}
