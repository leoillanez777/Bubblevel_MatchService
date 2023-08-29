using System;
using Microsoft.EntityFrameworkCore;

namespace Bubblevel_MatchService.Models
{
  [Keyless]
  public class EmailSetting
  {
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool UseSsl { get; set; }
    public bool UseTls { get; set; }
  }
}

