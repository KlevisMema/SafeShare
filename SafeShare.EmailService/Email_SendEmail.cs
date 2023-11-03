using SendGrid.Helpers.Mail;
using SendGrid;

namespace SafeShare.EmailService;

public static class Email_SendEmail
{
    public static async Task Execute()
    {
        var apiKey = Environment.GetEnvironmentVariable("SendGridKey");
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("klevis.mema@ulb.be", "Klevis Mema");
        var subject = "Sending with SendGrid is Fun";
        var to = new EmailAddress("memaklevis2@gmail.com", "Example User");
        var plainTextContent = "and easy to do anywhere, even with C#";
        var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);
    }
}