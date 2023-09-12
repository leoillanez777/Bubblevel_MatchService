using System;
using System.ComponentModel.DataAnnotations;

namespace Bubblevel_MatchService.Models
{
  public class Project
  {
    [Key]
    [Display(Name = "Code")]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Display(Name = "Initial Date")]
    public DateTime? IntialDate { get; set; }

    public string? Status { get; set; }

    [DataType(DataType.Duration)]
    public TimeSpan? Hours { get; set; }

    public bool Closed { get; set; } = false;

    public ICollection<SupportIncident>? SupportIncidents { get; set; } = null!;
  }
}

