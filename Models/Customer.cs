using System;
using System.ComponentModel.DataAnnotations;

namespace Bubblevel_MatchService.Models;

public class Customer
{
  [Key]
  public int Id { get; set; }
  [Required]
  public string Name { get; set; } = null!;
  [Required]
  public string Email { get; set; } = null!;
  public bool HasActiveSupportPlan { get; set; }
  public ICollection<SupportIncident> SupportIncidents { get; set; } = null!;
}

