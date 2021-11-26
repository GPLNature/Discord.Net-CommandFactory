using System.Collections.Generic;
using System.Collections.Immutable;

namespace CommandFactory.Info
{
  internal class SubModuleInfo : IModuleInfo
  {

    public SubModuleInfo(string name, string description, IModule module, IEnumerable<CommandInfo> subCommands,
      List<SubModuleInfo> subGroups) : base(name, description, module, subCommands, subGroups)
    {
    }
  }
}