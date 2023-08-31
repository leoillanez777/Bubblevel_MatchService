using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using Bubblevel_MatchService.Models;
using Bubblevel_MatchService.Services.Interfaces;
using Bubblevel_MatchService.Migrations;

namespace Bubblevel_MatchService.Services
{
  public class EmailSenderService : IEmailSender
  {
    private readonly EmailSetting _emailSettings;

    public EmailSenderService(IEmailSettingsRepository emailSettings)
    {
      _emailSettings = emailSettings.GetEmailSettingsAsync();
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
      var email = new MimeMessage();

      email.From.Add(new MailboxAddress("Sender Name", _emailSettings.Username));
      email.To.Add(new MailboxAddress("Receiver Name", toEmail));

      email.Subject = subject;
      email.Body = new TextPart(MimeKit.Text.TextFormat.Html) {
        Text = "<b>Hello all the way from the land of C#</b>"
      };

      using (var smtp = new SmtpClient()) {
        await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, _emailSettings.UseSsl);

        // Note: only needed if the SMTP server requires authentication
        await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
      }

    }
  }
}

