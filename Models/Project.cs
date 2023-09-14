using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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

    [Column(TypeName = "decimal(18, 2)")]
    [UIHint("Time")]
    public decimal Duration { get; set; } = 0;

    public bool Closed { get; set; } = false;

    public ICollection<SupportIncident>? SupportIncidents { get; set; } = null!;
  }
}

