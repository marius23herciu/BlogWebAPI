using BlogWebAPI.BusinessLayers;
using BlogWebAPI.Models.DTOs;
using BlogWebAPI.Services.EmailSender;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribersController : ControllerBase
    {
        private readonly SubscribersBusinessLayer subscribersBusinessLayer;

        public SubscribersController(SubscribersBusinessLayer subscribersBusinessLayer)
        {
            this.subscribersBusinessLayer = subscribersBusinessLayer;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> CreateSubscriber(SubDto subDto)
        {
            return Ok(await subscribersBusinessLayer.CreateSubscriber(subDto.Name, subDto.Email));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubscribers()
        {
            return Ok(await subscribersBusinessLayer.GetAllSubscribers());
        }

        [HttpDelete]
        [Route("{subToDelete}")]
        public async Task<ActionResult<bool>> DeleteSubscriber([FromRoute] Guid subToDelete)
        {
            var subscriberDeleted = await subscribersBusinessLayer.DeleteSubscriber(subToDelete);
            if (subscriberDeleted == false)
            {
                return Ok("Subscriber not found!");
            }
            return Ok(subscriberDeleted);
        }
    }
}
