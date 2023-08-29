using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bubblevel_MatchService.Models
{
  public class SupportIncident
  {
    [Key]
    [Display(Name = "Incident Number")]
    public int Id { get; set; }

    [Required]
    public string Summary { get; set; } = null!;

    [Column(TypeName = "decimal(18, 2)")]
    [Display(Name = "Cost Incurred")]
    public decimal? Total { get; set; }

    [ForeignKey("CustomerId")]
    [Display(Name = "Customer")]
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public ICollection<Intervention>? Interventions { get; set; }

    public ICollection<Comment>? Comments { get; set; }

    [ForeignKey("ProjectId")]
    [Display(Name = "Project")]
    public int ProjectId { get; set; }
    public Project? Project { get; set; }

    public string State { get; set; } = null!;
  }
}

