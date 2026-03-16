using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AuthenticationAuthorization.Services;

public class EmailService(IConfiguration config)
{
    public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
    {
        
        var resetUrl = $"{config["App:FrontendUrl"]}/reset-password?token={resetToken}";

        
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(config["Email:From"]));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = "Password Reset Request";
        email.Body = new TextPart("html")
        {
            Text = $"""
                <h2>Password Reset</h2>
                <p>You requested a password reset. Click the link below:</p>
                <a href="{resetUrl}">Reset Password</a>
                <p>This link expires in 10 minutes.</p>
                """
        };

        // Send email
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            config["Email:Host"],
            int.Parse(config["Email:Port"]!),
            SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            config["Email:Username"],
            config["Email:Password"]);

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}