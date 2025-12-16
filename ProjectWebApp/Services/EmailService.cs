using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress("Flightly", _config["EmailSettings:FromEmail"]));
        email.To.Add(new MailboxAddress("", toEmail));
        email.Subject = subject;

        email.Body = new TextPart("html")
        {
            Text = htmlMessage
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _config["EmailSettings:SmtpHost"],
            587,
            SecureSocketOptions.StartTls
        );

        await smtp.AuthenticateAsync(
            _config["EmailSettings:Username"],
            _config["EmailSettings:Password"]
        );

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
