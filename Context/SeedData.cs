using System;
using Microsoft.AspNetCore.Identity;

namespace Bubblevel_MatchService.Context
{
  public static class SeedData
  {
    public static async Task InitializeAsync(IServiceProvider services)
    {
      var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
      var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

      await EnsureRolesAsync(roleManager);
      await EnsureTestUserAsync(userManager);
    }

    private static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
    {
      foreach (var rol in Enum.GetValues<EnumRoles>()) {
        var nameRol = rol.ToString();
        var alreadyExists = await roleManager.RoleExistsAsync(nameRol);
        if (!alreadyExists) {
          await roleManager.CreateAsync(new IdentityRole(nameRol));
        }
      }
    }

    private static async Task EnsureTestUserAsync(UserManager<ApplicationUser> userManager)
    {
      var testUser = await userManager.FindByNameAsync("bubblevel@bubblevel.com");
      if (testUser == null) {
        testUser = new ApplicationUser {
          SourceView = "Index",
          FirstName = "Gonçalo",
          LastName = "Conde",
          UserName = "info@bubblevel.com",
          EmailConfirmed = true,
          Email = "info@bubblevel.com",
        };
        await userManager.CreateAsync(testUser, "Password123!");
      }
      await userManager.AddToRoleAsync(testUser, "SuperAdmin");
    }


    public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
      await roleManager.CreateAsync(new IdentityRole(EnumRoles.SuperAdmin.GetDisplayName()));
    }

  }
}

