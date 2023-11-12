using BlogWebAPI.Data;
using BlogWebAPI.Models.DTOs;
using BlogWebAPI.Models.Entities;
using BlogWebAPI.Services.EmailSender;
using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Xml.Linq;

namespace BlogWebAPI.BusinessLayers
{
    public class SubscribersBusinessLayer
    {
        private readonly BlogDbContext dbContext;
        private readonly ToDto toDtos;
        private readonly IEmailSender emailSender;

        public SubscribersBusinessLayer(BlogDbContext dbContext, ToDto toDto, IEmailSender emailSender)
        {
            this.dbContext = dbContext;
            this.toDtos = toDto;
            this.emailSender = emailSender;
        }

        public async Task<bool> CreateSubscriber(string subscribersName, string toEmailAddress)
        {
            //check email
            var checkEmail = await dbContext.Subscribers.Where(x => x.Email == toEmailAddress).FirstOrDefaultAsync();
            if (checkEmail != null)
            {
                return false;
            }

            var newSubscriber = new Subscriber()
            {
                Id = new Guid(),
                Name = subscribersName,
                Email = toEmailAddress,
            };

            await dbContext.Subscribers.AddAsync(newSubscriber);
            await dbContext.SaveChangesAsync();

            await emailSender.SendSubscriptionEmailAsync(toEmailAddress, subscribersName);

            return true;
        }

        

        public async Task<List<Subscriber>> GetAllSubscribers()
        {
            var allSubscribers = await dbContext.Subscribers.OrderBy(a => a.Name).ToListAsync();

            return allSubscribers;
        }

        public async Task<bool> DeleteSubscriber(Guid id)
        {

           var subToDelete = dbContext.Subscribers.Where(i => i.Id == id).FirstOrDefault();

            if (subToDelete == null)
            {
                return false;
            }

            dbContext.Subscribers.Remove(subToDelete);
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
