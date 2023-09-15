using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Bubblevel_MatchService; 
public static class EnumExtensions {
  public static string GetDisplayName(this Enum enumValue)
  {
    return enumValue.GetType()
      .GetMember(enumValue.ToString())
      .First()!
      .GetCustomAttribute<DisplayAttribute>()!
      .GetName()!;
  }

  public static string GetDescription(this Enum enumValue)
  {
    var enumType = enumValue.GetType();
    var member = enumType.GetMember(enumValue.ToString()).FirstOrDefault();

    if (member is not null) {
      var displayAttr = member.GetCustomAttribute<DisplayAttribute>();
      if (displayAttr is not null) {
        if (displayAttr.Description is not null) {
          return displayAttr.Description;
        }
      }
    }

    // Retornar valor por defecto si no se encuentra descripción
    return enumValue.ToString();
  }
}

