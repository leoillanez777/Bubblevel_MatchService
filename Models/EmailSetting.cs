using System;
using System.ComponentModel.DataAnnotations;

namespace Bubblevel_MatchService.Models
{
  public class EmailSetting
  {
    [Key]
    public int Id { get; set; }
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool UseSsl { get; set; }
    public bool UseTls { get; set; }
  }
}

