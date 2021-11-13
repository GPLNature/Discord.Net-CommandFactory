using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Discord;

namespace CommandFactory
{
  internal static class ParameterChecker
  {
    private static readonly ImmutableDictionary<Type, ApplicationCommandOptionType> Mapper =
      new Dictionary<Type, ApplicationCommandOptionType>
      {
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
      }.ToImmutableDictionary();

    public static ApplicationCommandOptionType? MappingType(ParameterInfo info)
    {
      if (Mapper.TryGetValue(info.ParameterType, out var result))
        return result;
      return null;
    }

    public static Type? ReverseMappingType(ApplicationCommandOptionType type)
    {
      var application = Mapper.SingleOrDefault(x => x.Value == type);
      return application.Equals(default(KeyValuePair<Type, ApplicationCommandOptionType>)) ? null : application.Key;
    }
  }
}