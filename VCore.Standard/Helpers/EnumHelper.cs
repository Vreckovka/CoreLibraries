using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace VCore.Standard.Helpers
{
  public static class EnumHelper
  {
    public class ValueDescription
    {
      public object Value { get; set; }
      public string Description { get; set; }
    }

    public static string Description(this Enum value)
    {
      var attributes = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
      if (attributes.Any())
        return (attributes.First() as DescriptionAttribute).Description;

      // If no description is found, the least we can do is replace underscores with spaces
      // You can add your own custom default formatting logic here
      TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
      return ti.ToTitleCase(ti.ToLower(value.ToString().Replace("_", " ")));
    }

    public static IEnumerable<ValueDescription> GetAllValuesAndDescriptions(Type t)
    {
      if (!t.IsEnum)
        throw new ArgumentException($"{nameof(t)} must be an enum type");

      var asd = Enum.GetValues(t).Cast<Enum>().Select((e) => new ValueDescription() { Value = e, Description = e.Description() }).ToList();

      return asd;
    }

    public static IEnumerable<Enum> GetAllValues(Type t)
    {
      if (!t.IsEnum)
        throw new ArgumentException($"{nameof(t)} must be an enum type");

      var asd = Enum.GetValues(t).Cast<Enum>().ToList();

      return asd;
    }
  }
}