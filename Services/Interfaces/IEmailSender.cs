using System;
using Bubblevel_MatchService.Models;

namespace Bubblevel_MatchService.Services.Interfaces
{
  public interface IEmailSender
  {
    /// <summary>
    /// Send email
    /// </summary>
    /// <param name="toEmail">email to receiver</param>
    /// <param name="subject"> subject of email</param>
    /// <param name="message">message to send</param>
    /// <param name="state">state of process</param>
    /// <param name="nameCustomer">name of customer</param>
    /// <param name="hash">if exists hash send button</param>
    /// <returns></returns>
    Task SendEmailAsync(string toEmail,
      string subject,
      string message,
      State state,
      string? nameCustomer,
      string? hash = null);

    Task SendEmailAsync(string email, string subject, string message, string name);
  }
}

