using System;
using Microsoft.AspNetCore.Identity;

namespace Bubblevel_MatchService.Areas.Identity.Data;

public class ApplicationUser : IdentityUser {
  public string SourceView { get; set; } = null!;
  [PersonalData]
  public string FirstName { get; set; } = null!;
  [PersonalData]
  public string LastName { get; set; } = null!;

  public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; } = null!;
}
