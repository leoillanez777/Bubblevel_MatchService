using System;
namespace Bubblevel_MatchService
{
  public static class Tools
  {
    public static string BuildStateText(int hour, int minute)
    {
      return $"{(hour > 0 ? hour + " hr" : "")} {minute} min".Trim();
    }

    public static string BuildAbbreviation(int hour, int minute)
    {
      var totalMinutes = hour * 60 + minute;
      return (totalMinutes / 60f).ToString("0.00");
    }
  }
}

