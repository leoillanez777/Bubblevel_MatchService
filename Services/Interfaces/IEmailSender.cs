using System;
namespace Bubblevel_MatchService.Services.Interfaces
{
  public interface IEmailSender
  {
    Task SendEmailAsync(string toEmail, string subject, string message);
  }
}

