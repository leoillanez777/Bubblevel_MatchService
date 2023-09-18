using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bubblevel_MatchService.Models
{
  public class Intervention {
    [Key]
    public int Id { get; set; }

    [Required]
    public string Description { get; set; } = null!;

    [DataType(DataType.DateTime)]
    [Display(Name = "Internetion Date")]
    [Required]
    public DateTime InterventionDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    [UIHint("Time")]
    public decimal Duration { get; set; }

    [ForeignKey("SupportIncidentId")]
    [Display(Name = "Support Incident")]
    public int SupportIncidentId { get; set; }
    public SupportIncident? SupportIncident { get; set; }
  }
}

