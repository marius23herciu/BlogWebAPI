using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using MimeKit;
using System.Net;
using System.Net.Mail;
using System.Security.Authentication;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;

namespace BlogWebAPI.Services.EmailSender
{
    public class EmailSender : IEmailSender
    {
        public async Task SendSubscriptionEmailAsync(string toEmailAddress, string subscribersName)
        {
            StringBuilder emailBody = new StringBuilder();
            emailBody.Append("Thank You, " + subscribersName);
            emailBody.Append(" for subscribing to MyBlog.");
            emailBody.AppendLine("<br>");
            emailBody.AppendLine("<br>");
            emailBody.AppendLine("We'll notify you when a new post is released.");
            emailBody.AppendLine("<br>");
            emailBody.AppendLine("<br>");
            emailBody.AppendLine("Stay tuned! :)");

            string emailSubject = "MyBlog Subscription Activated!";

            var mail = "myblog.test00@gmail.com";
            var password = "yrzyppydnfwtgjao";
            //var password = "MyBlog@00";

            MailMessage email = new MailMessage();
            email.From = new MailAddress(mail);
            email.Subject = emailSubject;
            email.To.Add(new MailAddress(toEmailAddress));

            email.Body = emailBody.ToString();
            email.IsBodyHtml = true;

            var client = new System.Net.Mail.SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(mail, password),
                EnableSsl = true
            };

            client.Send(email);

        }
        public async Task SendNewPostEmailAsync(string toEmailAddress, string subscribersName)
        {
            StringBuilder emailBody = new StringBuilder();
            emailBody.Append("Hi, " + subscribersName);
            emailBody.AppendLine("<br>"); emailBody.AppendLine("<br>");
            emailBody.AppendLine("We have posted a new MyBlog article.");
            emailBody.AppendLine("<br>");
            emailBody.AppendLine("Check it and tell us what you think about it.");
            emailBody.AppendLine("<br>"); emailBody.AppendLine("<br>");
            emailBody.AppendLine("Kind regards,");
            emailBody.AppendLine("<br>");
            emailBody.AppendLine("MyBlog Team");

            string emailSubject = "New MyBlog article!";

            var mail = "myblog.test00@gmail.com";
            var password = "yrzyppydnfwtgjao";
            //var password = "MyBlog@00";

            MailMessage email = new MailMessage();
            email.From = new MailAddress(mail);
            email.Subject = emailSubject;
            email.To.Add(new MailAddress(toEmailAddress));

            email.Body = emailBody.ToString();
            email.IsBodyHtml = true;

            var client = new System.Net.Mail.SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(mail, password),
                EnableSsl = true
            };

            client.Send(email);

        }
    }
}
