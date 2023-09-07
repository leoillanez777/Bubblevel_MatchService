using System;
using System.ComponentModel.DataAnnotations;

namespace Bubblevel_MatchService.Models
{
  public enum State
  {
    [Display(Name = "Pending Approval")]
    Pending,
    [Display(Name = "Rejected by the customer")]
    Rejected,
    [Display(Name = "On Hold")]
    OnHold,
    [Display(Name = "Awaiting Financial")]
    Awaiting,
    [Display(Name = "In Progress")]
    InProgress,
    [Display(Name = "Solved")]
    Solved,
  }
}

