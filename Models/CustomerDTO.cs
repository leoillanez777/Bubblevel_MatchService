using System;
using Newtonsoft.Json;

namespace Bubblevel_MatchService.Models
{
  public class CustomerDTO
  {
    [JsonProperty("name")]
    public string Name { get; set; } = null!;
    [JsonProperty("email")]
    public string Email { get; set; } = null!;
  }
}

