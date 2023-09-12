using System;
using Microsoft.AspNetCore.Identity;

namespace Bubblevel_MatchService
{
  public static class UserManagerExtensions
  {
    public static async Task<string> GetFullName(this UserManager<ApplicationUser> userManager, System.Security.Claims.ClaimsPrincipal user)
    {
      var applicationUser = await userManager.GetUserAsync(user);
      return applicationUser != null ? applicationUser.FullName() : "Guest";
    }

  }
}

