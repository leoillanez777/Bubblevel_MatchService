using System;
using System.ComponentModel.DataAnnotations;

namespace Bubblevel_MatchService.Models;

public class Customer
{
  [Key]
  public int Id { get; set; }
  [Required]
  public string Name { get; set; } = null!;
  public string Email { get; set; } = null!;
  public bool HasActiveSupportPlan { get; set; }
  public DateTime DateCreated { get; set; }
}

