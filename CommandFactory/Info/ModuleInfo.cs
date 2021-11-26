using System.Collections.Generic;
using System.Collections.Immutable;

namespace CommandFactory.Info
{
  internal class ModuleInfo : IModuleInfo
  {
    public readonly CommandInfo Executor;

    public ModuleInfo(string name, string description, IModule module, CommandInfo executor,
      IEnumerable<CommandInfo> subCommands, List<SubModuleInfo> subGroups) : base(name, description, module, subCommands, subGroups)
    {
      Executor = executor;
    }
  }
}