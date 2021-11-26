using System.Collections.Generic;
using System.Collections.Immutable;

namespace CommandFactory.Info
{
  internal class IModuleInfo
  {
    public readonly string Name;
    public readonly string Description;
    public readonly IModule Module;
    public readonly ImmutableDictionary<string, CommandInfo> SubCommands;
    public readonly ImmutableDictionary<string, SubModuleInfo> SubGroups;

    public IModuleInfo(string name, string description, IModule module, IEnumerable<CommandInfo> subCommands,
      IEnumerable<SubModuleInfo> subGroups)
    {
      Name = name;
      Description = description;
      Module = module;
      SubCommands = subCommands.ToImmutableDictionary(x => x.Name);
      SubGroups = subGroups.ToImmutableDictionary(x => x.Name);
    }
  }
}