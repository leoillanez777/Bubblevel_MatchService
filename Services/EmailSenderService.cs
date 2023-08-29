using System;
using System.Net;
using System.Net.Mail;
using Bubblevel_MatchService.Models;
using Bubblevel_MatchService.Services.Interfaces;

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
      using var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port);
      client.UseDefaultCredentials = false;
      client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
      client.EnableSsl = _emailSettings.UseSsl;

      var mailMessage = new MailMessage {
        From = new MailAddress(_emailSettings.Username),
        Subject = subject,
        Body = message,
        IsBodyHtml = true
      };
      mailMessage.To.Add(toEmail);

      await client.SendMailAsync(mailMessage);

    }
  }
}

