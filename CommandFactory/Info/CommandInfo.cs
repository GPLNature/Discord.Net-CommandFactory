﻿using System.Collections.Generic;
using System.Reflection;

namespace CommandFactory.Info
{
  internal readonly struct CommandInfo
  {
    public readonly string Name;
    public readonly string Description;
    public readonly MethodInfo Method;
    public readonly List<ParameterInfo> Parameters;

    public CommandInfo(string name, string description, MethodInfo method, List<ParameterInfo> parameters)
    {
      Name = name;
      Description = description;
      Method = method;
      Parameters = parameters;
    }
  }
}