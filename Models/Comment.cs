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
    public string Text { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    [ForeignKey("SupportIncidentId")]
    public int SupportIncidentId { get; set; }

    public SupportIncident? SupportIncident { get; set; }
  }
}

