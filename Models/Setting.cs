using System;
using System.ComponentModel.DataAnnotations;

namespace Bubblevel_MatchService.Models
{
  public class Setting
  {
    [Key]
    public int Id { get; set; }

    public State State { get; set; } = State.Pending;

    [Display(Name = "Delivery Type")]
    public Emailing DeliveryService { get; set; } = Emailing.CustomerOnly;

    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string? EmailSender { get; set; }

    [Display(Name = "Email Originator Name")]
    public string? Name { get; set; }

    [Display(Name = "Email Receiver Name")]
    public string? NameReceiver { get; set; }

    [Display(Name = "Text of Message")]
    public string? Summary { get; set; }

    [Display(Name = "Response by Email")]
    public bool ResponseByEmail { get; set; }
  }
}

