// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Bubblevel_MatchService.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Bubblevel_MatchService.Areas.Identity.Pages.Account.Manage {

  [Authorize(Roles = "Admin")]
  public class RoleModel : PageModel {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RoleModel> _logger;

    public RoleModel(
        UserManager<ApplicationUser> userManager,
        ILogger<RoleModel> logger)
    {
      _userManager = userManager;
      _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public class InputModel {
      
      [Display(Name = "Roles")]
      public List<RolesUser> ListRoles { get; set; }
      public string UserName { get; set; }
      public string UserID { get; set; }
    }

    public class RolesUser
    {
      public EnumRoles Rol { get; set; }
      public bool Active { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string userId = null)
    {
      var user = userId is not null ?
        await _userManager.FindByIdAsync(userId) :
        await _userManager.GetUserAsync(User);

      if (user == null) {
        return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
      }

      Input = new() {
        ListRoles = new(),
        UserName = user.UserName,
        UserID = userId
      };

      var roles = await _userManager.GetRolesAsync(user);
      foreach (var rol in Enum.GetValues<EnumRoles>()) {
        Input.ListRoles.Add(new RolesUser {
          Rol = rol,
          Active = roles.Any(r => r == rol.ToString())
        });
      }

      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid) {
        return Page();
      }

      var user = Input.UserID is not null ?
        await _userManager.FindByIdAsync(Input.UserID) :
        await _userManager.GetUserAsync(User);

      if (user == null) {
        return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
      }

      var rolesAdd = Input.ListRoles.Where(l => l.Active).Select(l => l.Rol.ToString());
      foreach(var rol in rolesAdd) {
        await _userManager.AddToRoleAsync(user, rol);
      }

      var rolesRemove = Input.ListRoles.Where(l => !l.Active).Select(l => l.Rol.ToString());

      var result = await _userManager.RemoveFromRolesAsync(user, rolesRemove);
      if (!result.Succeeded) {
        foreach (var rol in rolesRemove) {
          await _userManager.RemoveFromRoleAsync(user, rol);
        }
      }
      
      _logger.LogInformation("The user successfully changed roles.");
      StatusMessage = "The user successfully changed roles.";
      if (Input.UserID is not null) {
        return RedirectToPage(new { userId = Input.UserID });
      }

      return RedirectToPage();
    }
  }
}
