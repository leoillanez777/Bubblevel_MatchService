using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bubblevel_MatchService.Models;

public class Customer
{
  [Key]
  public int Id { get; set; }
  [Required]
  public string Name { get; set; } = null!;
  [Required]
  public string Email { get; set; } = null!;
  [Display(Name = "Has Active Support Plan")]
  public bool HasActiveSupportPlan { get; set; }
  public ICollection<SupportIncident>? SupportIncidents { get; set; }
}

