using System;
using Discord;

namespace CommandFactory.Info
{
  internal readonly struct ParameterInfo
  {
    public readonly string Name;
    public readonly string Description;
    public readonly ApplicationCommandOptionType OptionType;
    public readonly Type Type;
    public readonly object? DefaultValue;
    public readonly bool IsRequire;

    public ParameterInfo(string name, string description, ApplicationCommandOptionType optionType,
      object? defaultValue, Type type, bool isRequire)
    {
      Name = name;
      Description = description;
      OptionType = optionType;
      DefaultValue = defaultValue;
      Type = type;
      IsRequire = isRequire;
    }
  }
}