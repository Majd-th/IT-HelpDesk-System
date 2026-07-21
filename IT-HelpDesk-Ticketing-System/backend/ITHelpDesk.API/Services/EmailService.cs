using ITHelpDesk.API.Configuration;
using ITHelpDesk.API.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace ITHelpDesk.API.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetLink)
    {
        using var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
        {
            Credentials = new NetworkCredential(
                _settings.Username,
                _settings.Password),

            EnableSsl = true
        };

        var message = new MailMessage
        {
            From = new MailAddress(_settings.From),
            Subject = "Reset your IT HelpDesk password",
            Body =
$@"Hello,

We received a request to reset your password.

Click the link below to reset it:

{resetLink}

If you did not request this password reset, you can safely ignore this email.

IT HelpDesk Team",
            IsBodyHtml = false
        };

        message.To.Add(email);

        await client.SendMailAsync(message);
    }
}