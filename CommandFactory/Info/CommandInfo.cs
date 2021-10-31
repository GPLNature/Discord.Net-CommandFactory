using System.Collections.Generic;
using System.Reflection;

namespace CommandFactory.Info
{
  internal readonly struct CommandInfo
  {
    public readonly string Name;
    public readonly string Description;
    public readonly MethodInfo Method;
    public readonly List<ParameterInfo> Parameters;
    public readonly CommandType Type;

    public CommandInfo(string name, string description, MethodInfo method, List<ParameterInfo> parameters, CommandType type)
    {
      Name = name;
      Description = description;
      Method = method;
      Parameters = parameters;
      Type = type;
    }
  }
}