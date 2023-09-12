using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bubblevel_MatchService.Areas.Identity.Data;

public class ApplicationUser : IdentityUser {
  [TempData]
  public string? SourceView { get; set; }

  [PersonalData]
  [Required]
  [Display(Name="First Name")]
  public string FirstName { get; set; } = null!;

  [PersonalData]
  [Required]
  [Display(Name = "Last Name")]
  public string LastName { get; set; } = null!;

  [PersonalData]
  public byte[]? ProfilePicture { get; set; }

  public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; } = null!;

  public string FullName() => $"{FirstName} {LastName}";
}
