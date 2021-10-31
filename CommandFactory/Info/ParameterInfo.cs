using Discord;

namespace CommandFactory.Info
{
  internal readonly struct ParameterInfo
  {
    public readonly string Name;
    public readonly string Description;
    public readonly ApplicationCommandOptionType OptionType;

    public ParameterInfo(string name, string description, ApplicationCommandOptionType optionType)
    {
      Name = name;
      Description = description;
      OptionType = optionType;
    }
  }
}