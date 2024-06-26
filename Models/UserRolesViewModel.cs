﻿using System;
namespace Bubblevel_MatchService.Models;

public class UserRolesViewModel {

  public string UserId { get; set; } = null!;
  public string FirstName { get; set; } = null!;
  public string LastName { get; set; } = null!;
  public string UserName { get; set; } = null!;
  public string? Email { get; set; }
  public IEnumerable<string> Roles { get; set; } = null!;

}

