using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Discord;

namespace CommandFactory
{
  internal class ParameterMappingException : Exception
  {
    public ParameterMappingException()
    {
    }

    public ParameterMappingException(string? message) : base(message)
    {
    }

    public ParameterMappingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
  }
  
  internal class ParameterChecker
  {
    private static readonly ReadOnlyDictionary<Type, ApplicationCommandOptionType> Mapper = new(new Dictionary<Type, ApplicationCommandOptionType> {
        { typeof(string), ApplicationCommandOptionType.String },
        { typeof(int), ApplicationCommandOptionType.Integer },
        { typeof(bool), ApplicationCommandOptionType.Boolean },
        { typeof(IUser), ApplicationCommandOptionType.User },
        { typeof(IGuildChannel), ApplicationCommandOptionType.Channel },
        { typeof(IRole), ApplicationCommandOptionType.Role },
        { typeof(IMentionable), ApplicationCommandOptionType.Mentionable },
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