using System;
namespace Bubblevel_MatchService.Models
{
  public class PaginationViewModel
  {
    public string ActionName { get; set; } = null!;
    public object Model { get; set; } = new();
  }
}

