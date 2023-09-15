using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bubblevel_MatchService.Context;
using Bubblevel_MatchService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bubblevel_MatchService.Controllers;

[Authorize(Roles = "SuperAdmin")]
public class ManageUserController : Controller {

  private readonly ILogger<HomeController> _logger;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly RoleManager<IdentityRole> _roleManager;

  public ManageUserController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
  {
    _logger = logger;
    _userManager = userManager;
    _roleManager = roleManager;
  }

  // GET: /ManageUser/
  public async Task<IActionResult> Index()
  {
    var users = await _userManager.Users.ToListAsync();
    var userRolesViewModel = new List<UserRolesViewModel>();

    foreach (ApplicationUser user in users) {
      var thisViewModel = new UserRolesViewModel
      {
        UserId = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Roles = await GetUserRoles(user)
      };
      userRolesViewModel.Add(thisViewModel);
    }

    return View(userRolesViewModel);
  }

  private async Task<List<string>> GetUserRoles(ApplicationUser user)
  {
    return new List<string>(await _userManager.GetRolesAsync(user));
  }
}
