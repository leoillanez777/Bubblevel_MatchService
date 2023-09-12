using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using Bubblevel_MatchService.Models;
using Bubblevel_MatchService.Services.Interfaces;
using System.Security.Policy;

namespace Bubblevel_MatchService.Services
{
  public class EmailSenderService : IEmailSender
  {
    private readonly IEmailSettingsRepository _emailSettingsRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly EmailSetting _emailSetting;
    private readonly string _appUrl;

    public EmailSenderService(IEmailSettingsRepository emailSettings, IHttpContextAccessor httpContextAccessor)
    {
      _emailSettingsRepository = emailSettings;
      _emailSetting = _emailSettingsRepository.GetEmailSettings();
      _httpContextAccessor = httpContextAccessor;
      _appUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
    }
    // HACK: Delete this
    // 1) Se envia email al cliente con espera de aprovación. (Pending)
    // 2) Cuando se aprueba y no tiene plan de soporte, se envia email a finanzas y al cliente. (Awaiting)
    // 3) Si no tiene nada pendiente se envia email a comercial. (OnHold)
    // 4) Incidencia resuelta, se envia al cliente con total (Solved)

    public async Task SendEmailAsync(string toEmail,
      string subject,
      string message,
      State state,
      string? nameCustomer,
      string? hash = null)
    {
      var settingForEmail = _emailSettingsRepository.GetSetting(state);
      var nameSender = settingForEmail.Name ?? "Bubblevel";
      var nameReceiver = settingForEmail.NameReceiver ?? nameCustomer ?? "Dear customer";
      var responseByEmail = settingForEmail.ResponseByEmail;
      var body = settingForEmail.Summary ?? "";

      using var email = new MimeMessage();
      email.From.Add(new MailboxAddress(nameSender, _emailSetting.Username));
      email.To.Add(new MailboxAddress(nameReceiver, toEmail));
      email.Subject = subject;

      email.Body = new TextPart(MimeKit.Text.TextFormat.Html) {
        Text = BuildMessageBody(toEmail, message, body, responseByEmail, hash)
      };

      using var smtp = new SmtpClient(); 
      await smtp.ConnectAsync(_emailSetting.Host, _emailSetting.Port, _emailSetting.UseSsl);

      try {
        await smtp.AuthenticateAsync(_emailSetting.Username, _emailSetting.Password);
        await smtp.SendAsync(email);
      }
      catch (Exception ex) {
        // TODO: middleware exceptions.
        throw new Exception(ex.Message, ex);
      }
      finally {
        await smtp.DisconnectAsync(true);
      }
    }

    public async Task SendEmailAsync(string email, string subject, string message, string name)
    {
      using var mimeMessage = new MimeMessage();
      mimeMessage.From.Add(new MailboxAddress("Bubblevel", _emailSetting.Username));
      mimeMessage.To.Add(new MailboxAddress(name, email));
      mimeMessage.Subject = subject;
      mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) {
        Text = message
      };

      using var smtp = new SmtpClient();
      await smtp.ConnectAsync(_emailSetting.Host, _emailSetting.Port, _emailSetting.UseSsl);

      try {
        await smtp.AuthenticateAsync(_emailSetting.Username, _emailSetting.Password);
        await smtp.SendAsync(mimeMessage);
      }
      catch (Exception ex) {
        // TODO: middleware exceptions.
        throw new Exception(ex.Message, ex);
      }
      finally {
        await smtp.DisconnectAsync(true);
      }
    }

    private string BuildMessageBody(string toEmail, string message, string body, bool resposeByEmail, string? hash)
    {
      var verifyUrl = $"{_appUrl}/validate?hash={hash}&email={toEmail}";
      var textBody = $"<p>{message}</p><p>{body}</p>";
      if (!string.IsNullOrEmpty(hash) && resposeByEmail) {
        textBody += $"<p><a href='{verifyUrl}'>Please confirm</a></p>";
      }
      return textBody;
    }
  }
}

