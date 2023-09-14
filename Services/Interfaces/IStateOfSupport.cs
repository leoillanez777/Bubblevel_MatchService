using System;
namespace Bubblevel_MatchService.Services.Interfaces
{
  public interface IStateOfSupport
  {
    Task<string> GetUrlForStateAsync(int supportId); 
  }
}

