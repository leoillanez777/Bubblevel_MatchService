using System;
using Bubblevel_MatchService.Models;

namespace Bubblevel_MatchService.Services.Interfaces
{
  public interface IEmailSettingsRepository
  {
    EmailSetting GetEmailSettings();
    Setting GetSetting(State state);
  }
}

