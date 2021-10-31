using System.Collections.Generic;
using System.Reflection;

namespace CommandFactory
{
  internal class ModuleBuilder
  {
    private Dictionary<string, MethodInfo> commands = new();

    public ModuleBuilder()
    {
    }

    public void AddCommand(string name, MethodInfo methodInfo)
    {
      commands.Add(name, methodInfo);
    }
  }
}