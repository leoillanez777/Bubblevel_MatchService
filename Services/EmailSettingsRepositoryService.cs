using System;
using Bubblevel_MatchService.Context;
using Bubblevel_MatchService.Models;
using Bubblevel_MatchService.Services.Interfaces;

namespace Bubblevel_MatchService.Services
{
  public class EmailSettingsRepositoryService : IEmailSettingsRepository {

    private readonly ApplicationDbContext _dbContext;

    public EmailSettingsRepositoryService(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public EmailSetting GetEmailSettingsAsync()
    {
      var emailSetting = _dbContext.EmailSetting.FirstOrDefault();
      return emailSetting!;
    }
  }
}

