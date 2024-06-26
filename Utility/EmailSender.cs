using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using PostmarkDotNet;

namespace Utility
{
    public class EmailSender : IEmailSender
    {
        public string PostMarkSecret { get; set; }
        public EmailSender(IConfiguration _config)
        {
            PostMarkSecret = _config.GetValue<string>("PostMark:SecretKey");
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new PostmarkMessage()
            {
                To = email,
                From = "huntermurphy@mail.weber.edu",
                TrackOpens = true,
                Subject = subject,
                HtmlBody = htmlMessage
            };
            var client = new PostmarkClient(PostMarkSecret);
            var sendResult = await client.SendMessageAsync(message);

            if (sendResult.Status == PostmarkStatus.Success) { return; }
            else
            {
                // Just a basic error message:
                throw new Exception($"Failed to send email. Status: {sendResult.Status}, Message: {sendResult.Message}");
            }
        }
    }
}