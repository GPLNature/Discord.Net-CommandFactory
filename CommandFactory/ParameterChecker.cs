using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Discord;

namespace CommandFactory
{
  internal class ParameterChecker
  {
    private static readonly ReadOnlyDictionary<Type, ApplicationCommandOptionType> Mapper = new(new Dictionary<Type, ApplicationCommandOptionType> {
        { typeof(string), ApplicationCommandOptionType.String },
        { typeof(int), ApplicationCommandOptionType.Integer },
        { typeof(uint), ApplicationCommandOptionType.Integer },
        { typeof(long), ApplicationCommandOptionType.Integer },
        { typeof(bool), ApplicationCommandOptionType.Boolean },
        { typeof(IUser), ApplicationCommandOptionType.User },
        { typeof(IGuildChannel), ApplicationCommandOptionType.Channel },
        { typeof(IRole), ApplicationCommandOptionType.Role },
        { typeof(IMentionable), ApplicationCommandOptionType.Mentionable },
        { typeof(float), ApplicationCommandOptionType.Number },
        { typeof(double), ApplicationCommandOptionType.Number },
      });
    
    public static ApplicationCommandOptionType? MappingType(ParameterInfo info)
    {
      if (Mapper.TryGetValue(info.ParameterType, out var result))
        return result;
      return null;
    }
  }
}