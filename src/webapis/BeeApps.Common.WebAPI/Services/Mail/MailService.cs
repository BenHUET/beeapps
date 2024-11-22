using BeeApps.Common.Models;
using BeeApps.Common.WebAPI.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace BeeApps.Common.Services;

public class MailService : IMailService
{
    private readonly MailOptions _mailOptions;

    public MailService(IOptions<MailOptions> mailOptions)
    {
        _mailOptions = mailOptions.Value;
    }

    public async Task Send(Mail mail)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_mailOptions.FromName, _mailOptions.FromAddress));
        message.To.Add(new MailboxAddress(mail.ToName, mail.ToAddress));
        message.Subject = mail.Subject;
        message.Body = new TextPart(TextFormat.Html) { Text = mail.Body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_mailOptions.Host, _mailOptions.Port, _mailOptions.UseSSL);
        await client.AuthenticateAsync(_mailOptions.Username, _mailOptions.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}