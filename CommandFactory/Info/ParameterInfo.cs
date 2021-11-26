using System;
using CommandFactory.Attributes;
using Discord;

namespace CommandFactory.Info
{
  internal class Options
  {
    public readonly bool IsAutoComplete;
    public readonly bool IsRequire;
    public readonly bool IsDefault;

    public Options(OptionAttribute? attr)
    {
      IsAutoComplete = attr?.IsAutoComplete ?? false;
      IsRequire = attr?.IsRequire ?? false;
      IsDefault = attr?.IsDefault ?? false;
    }
  }

  internal readonly struct ParameterInfo
  {
    public readonly string Name;
    public readonly string Description;
    public readonly ApplicationCommandOptionType OptionType;
    public readonly Type Type;
    public readonly object? DefaultValue;
    public readonly Options Options;

    public ParameterInfo(string name, string description, ApplicationCommandOptionType optionType,
      object? defaultValue, Type type, Options options)
    {
      Name = name;
      Description = description;
      OptionType = optionType;
      DefaultValue = defaultValue;
      Type = type;
      Options = options;
    }
  }
}