using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bubblevel_MatchService.Models
{
  public class Comment
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Comment")]
    public string Text { get; set; } = null!;

    [Display(Name = "Created At")]
    public DateTime? CreatedAt { get; set; }

    [Display(Name = "Created By")]
    public string? CreatedBy { get; set; }

    [ForeignKey("SupportIncidentId")]
    public int SupportIncidentId { get; set; }

    public SupportIncident? SupportIncident { get; set; }
  }
}

