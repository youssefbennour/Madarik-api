using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using IEmailSender = Microsoft.AspNetCore.Identity.UI.Services.IEmailSender;

namespace Madarik.Common.Emailing.Mimekit;

internal sealed class EmailSender(ILogger<EmailSender> logger,
EmailOptions emailOptions) : IEmailSender

{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
     
        var message = new MimeMessage()
        {
            From = {MailboxAddress.Parse(emailOptions.From)},
            To = {MailboxAddress.Parse(email)},
            Subject = subject,
            Body = new BodyBuilder
            {
                HtmlBody = htmlMessage
            }.ToMessageBody()
        };
        
        using var smtpClient = new SmtpClient();
        try
        {
            await smtpClient.ConnectAsync(
                emailOptions.Server, 
                emailOptions.Port, 
                MailKit.Security.SecureSocketOptions.StartTls);

            await smtpClient.AuthenticateAsync(emailOptions.From, emailOptions.Password); 
            await smtpClient.SendAsync(message);
            await smtpClient.DisconnectAsync(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,"An error occured while sending an email");
        }
    }
}