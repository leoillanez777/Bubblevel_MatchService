using System;
using Bubblevel_MatchService.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Bubblevel_MatchService.Extensions;

public static class IdentityExtensions {
  public static async Task<string?> GetClaim(this ApplicationUser user, UserManager<ApplicationUser> userManager, string nameClaim)
  {
    var claims = await userManager.GetClaimsAsync(user);
    return claims.FirstOrDefault(x => x.Type == nameClaim)?.Value;
  }
}
