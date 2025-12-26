using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


public class EmailService
{
    private readonly IConfiguration _config;//IConfiguration gives access to appsettings.json, Used to read email credentials safely

    public EmailService(IConfiguration config)
    {
        _config = config;//Constructor injection, ASP.NET provides configuration automatically
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage) //Sending Email Logic, Sends: recipient email, Subject, and HTML body
    {
        var username = _config["EmailSettings:Username"];//Read email settings from configuration, Used to get email service credentials from appsettings.json
        var password = _config["EmailSettings:Password"];
        var host = _config["EmailSettings:SmtpHost"];
        var from = _config["EmailSettings:FromEmail"];

        // ✅ SAFETY CHECK — prevents crashes
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(host) ||
            string.IsNullOrWhiteSpace(from))
        {
            // Optional: log for debugging
            Console.WriteLine("Email service disabled (missing configuration).");
            return;
        }

        //Email sending logic
        var email = new MimeMessage();// Create a new email message

        email.From.Add(new MailboxAddress("Flightly", _config["EmailSettings:FromEmail"]));//Sets sender, receiver, subject
        email.To.Add(new MailboxAddress("", toEmail));
        email.Subject = subject;

        email.Body = new TextPart("html")//Enables HTML emails
        {
            Text = htmlMessage
        };

        using var smtp = new SmtpClient(); //Creates SMTP client
        await smtp.ConnectAsync(
            _config["EmailSettings:SmtpHost"],
            587,
            SecureSocketOptions.StartTls
        );//Secure connection using TLS

        await smtp.AuthenticateAsync(
            _config["EmailSettings:Username"],
            _config["EmailSettings:Password"]
        );//Authenticates email account

        await smtp.SendAsync(email);//Sends the email
        await smtp.DisconnectAsync(true);//Disconnects from SMTP server,Closes connection safely
    }
}
