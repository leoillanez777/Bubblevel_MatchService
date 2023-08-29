using System;
using System.ComponentModel.DataAnnotations;

namespace Bubblevel_MatchService.Models
{
  public class AuditLog
  {
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string Action { get; set; } = null!;
    public string Entity { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public string? AffectedDataJson { get; set; }
  }
}

