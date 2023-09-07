using System;
using System.ComponentModel.DataAnnotations;

namespace Bubblevel_MatchService.Models
{
  public enum Emailing
  {
    [Display(Name = "Nothing")]
    Nothing,
    [Display(Name = "Customer Only")]
    CustomerOnly,
    [Display(Name = "Internal")]
    Internal,
    [Display(Name = "Internal and Customer")]
    InternalAndCustomer,
  }
}

