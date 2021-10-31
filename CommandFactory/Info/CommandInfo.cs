using System.Collections.Generic;

namespace CommandFactory.Info
{
  internal class CommandInfo
  {
    public readonly string Name;
    public readonly string Description;
    public readonly List<ParameterInfo> parameters;
  }
}