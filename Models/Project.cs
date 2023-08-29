using System;
using System.ComponentModel.DataAnnotations;

namespace Bubblevel_MatchService.Models
{
  public class Project
  {
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public string Status { get; set; } = null!;

    public ICollection<SupportIncident> SupportIncidents { get; set; } = null!;
  }
}

