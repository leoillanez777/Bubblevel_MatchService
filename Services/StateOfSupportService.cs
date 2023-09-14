using System;
using Bubblevel_MatchService.Context;
using Bubblevel_MatchService.Models;
using Bubblevel_MatchService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Bubblevel_MatchService.Services
{
  public class StateOfSupportService : IStateOfSupport
  {

    private readonly ApplicationDbContext _dbContext;

    public StateOfSupportService(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task<string> GetUrlForStateAsync(int supportId)
    {
      var incident = await _dbContext.SupportIncident.FindAsync(supportId);
      string returnUrl = nameof(Controllers.SupportIncidentController.Index);
      switch (incident!.State) {
        case State.Awaiting:
        case State.OnHold:
          returnUrl = nameof(Controllers.SupportIncidentController.AwaitingList);
          break;
        case State.InProgress:
          returnUrl = nameof(Controllers.SupportIncidentController.ListInProgress);
          break;
        case State.Solved:
          returnUrl = nameof(Controllers.SupportIncidentController.SolvedList);
          break;
      }
      return returnUrl;
    }
  }
}

