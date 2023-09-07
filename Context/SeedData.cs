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
      var alreadyExists = await roleManager.RoleExistsAsync("Admin");
      if (!alreadyExists) {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
      }
    }

    private static async Task EnsureTestUserAsync(UserManager<ApplicationUser> userManager)
    {
      var testUser = await userManager.FindByNameAsync("test@example.com");
      if (testUser == null) {
        testUser = new ApplicationUser {
          UserName = "test@example.com",
          Email = "test@example.com",
        };
        await userManager.CreateAsync(testUser, "Password123!");
      }
      await userManager.AddToRoleAsync(testUser, "Admin");
    }
  }
}

