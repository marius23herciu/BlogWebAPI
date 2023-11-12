namespace BlogWebAPI.Services.EmailSender
{
    public interface IEmailSender
    {
        Task SendSubscriptionEmailAsync(string toEmailAddress, string subscribersName);
        Task SendNewPostEmailAsync(string toEmailAddress, string subscribersName);
    }
}
